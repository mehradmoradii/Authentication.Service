using Auth.Command.Commands.AppUsers;
using Auth.Command.Handler.Helpers;
using Auth.Command.Responses.AppUsers;
using Auth.Domain.AppUsers.Aggregate;
using Auth.Domain.AppUsers.Entity;
using Auth.Infrastracture.ErrorHandler;
using Auth.Infrastracture.Messages.Commands;
using Auth.Infrastracture.TokenServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Auth.Command.Handler.AppUsers
{
    public class AppUserCommandHandler : 
        ICommandHandler<RegisterAppUserCommand, RegisterAppUserCommandResponse>,
        ICommandHandler<EmailConfirmationCommand, EmailConfirmationCommandResponse>,
        ICommandHandler<LoginWithUserNameCommand, LoginUserCommandResponse>,
        ICommandHandler<ResendEmailConfirmationCommand>,
        //ICommandHandler<UploadImageProfileCommand>,
        ICommandHandler<LogoutUserCommand>,
        ICommandHandler<UpdateAppUserCommand>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IDatabase _redis;
        private readonly IHttpContextAccessor _http;
        private readonly IWebHostEnvironment _env;

        public AppUserCommandHandler(ITokenService tokenService, UserManager<AppUser> userManager,
            IConnectionMultiplexer redis, IHttpContextAccessor http, IWebHostEnvironment env)
        {
            _env = env;
            _tokenService = tokenService;
            _userManager = userManager;
            _redis = redis.GetDatabase();
            _http = http;
          
        }
        public async Task<RegisterAppUserCommandResponse> Handle(RegisterAppUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null && user.IsConfirm == false)
            {
                throw new AppException("There is an User with this Email, please confirm your emain and login");
            }

            user = new AppUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.UserName,
                IsConfirm = false,
                Claims = new List<AppClaim>()
            };

           
            var newClaim = new AppClaim { ClaimType= ClaimTypes.Email, ClaimValue=request.Email ,CreationDateTime = DateTime.Now};

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new AppException(string.Join(", ", result.Errors.Select(e => e.Description)));
            var otp = new Random().Next(100000, 999999).ToString();


            var emailText = @"
                    <!DOCTYPE html>
                    <html lang=""en"">
                    <body style=""font-family: Arial, sans-serif; background:#f9f9f9; padding:20px;"">
                        <div style=""
                            max-width:520px; 
                            margin:auto; 
                            background:white; 
                            padding:30px; 
                            border-radius:12px; 
                            box-shadow:0 4px 10px rgba(0,0,0,0.08);
                        "">
                            <h2 style=""margin:0; color:#2c3e50; text-align:center;"">🔐 Email Verification</h2>

                            <p style=""color:#555; font-size:15px; margin-top:18px; line-height:1.6;"">
                                Hello,<br><br>
                                Use the verification code below to complete your authentication:
                            </p>

                            <div style=""text-align:center; margin:25px 0;"">
                                <span style=""
                                    display:inline-block; 
                                    font-size:32px; 
                                    letter-spacing:6px; 
                                    padding:14px 30px; 
                                    background:#eef2ff; 
                                    border-radius:8px; 
                                    color:#111; 
                                    font-weight:bold;
                                "">
                                    {{otp}}
                                </span>
                            </div>

                            <p style=""color:#666; font-size:14px; line-height:1.6;"">
                                This code is valid for <strong>2 minutes</strong>.<br>
                                Never share this code with anyone.
                            </p>

                            <hr style=""border:0; border-top:1px solid #eee; margin:25px 0;"">

                            <p style=""margin:0; font-size:13px; color:#999; text-align:center;"">
                                Thank you,<br>
                                <strong>Your Authentication Team</strong>
                            </p>
                        </div>
                    </body>
                    </html>";


            emailText = emailText.Replace("{{otp}}", otp);
            Console.WriteLine(otp);
            // Store in session
            var session = _http.HttpContext.Session;
            var identityToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            
            session.SetString($"EMAIL_TOKEN:{user.Email}", identityToken);
            session.SetString($"OTP:{user.Email}", otp);
            session.SetString($"OTP_EXPIRE:{user.Email}", DateTime.Now.AddMinutes(2).ToString());
            
            return new RegisterAppUserCommandResponse
            {
                Message = emailText,
                Email = user.Email
            };

        }

        public async Task<EmailConfirmationCommandResponse> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var session = _http.HttpContext.Session;
            var StoredOtp = session.GetString($"OTP:{request.Email}");
            if (StoredOtp != request.OtpCode)
                throw new AppException("Invalid Code!!");
            var ExpireTimeString = session.GetString($"OTP_EXPIRE:{request.Email}");
            var ExpireTime = DateTime.TryParse(ExpireTimeString, out var ExpireTimeDate);
            if (ExpireTime && DateTime.Now > ExpireTimeDate)
                throw new Exception("Code expired");
            var identityToken = session.GetString($"EMAIL_TOKEN:{request.Email}");

            var user = await _userManager.FindByEmailAsync(request.Email);

            var confirmResult = await _userManager.ConfirmEmailAsync(user, identityToken);

            if (!confirmResult.Succeeded)
                throw new Exception("Email confirmation failed");
            user.EmailConfirmed = true;
            user.IsConfirm = true;
         
            var ip = _http.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var token = await _tokenService.GenerateTokenPairAsync(user, ip);
            var result = new EmailConfirmationCommandResponse
            {
                Token = token.AccessToken,
                IsConfirmed = true,
            };



            var key = $"refresh_token:{user.Id}:{token.RefreshToken}";
            var data = new
            {
                UserId = user.Id,
                RefreshToken = token.RefreshToken,
                Ip = ip,
                Expiration = token.RefreshTokenExpiry.ToString("O")
            };

            var json = JsonSerializer.Serialize(data);

            var ttl = token.RefreshTokenExpiry - DateTime.Now;
            await _redis.StringSetAsync(key, json, ttl);
            var login = new AppLogin()
            {
                IP = ip,
                UserId = user.Id,
                LoginProvider = request.UserAgent,
                ProviderKey = user.Id.ToString(),
                IsActive = true,
                DisplayName = LoginHelper.GenerateDisplayName(request.UserAgent),
                CreationDateTime = DateTime.Now,
                IsLoggedIn = true,

            };
            var profile = new AppProfileImage()
            {
                UserId = user.Id
            };
            user.Profile = profile;
            user.Logins.Add(login);
            user.Status = Infrastracture.Enums.UserState.Active;
            await _userManager.UpdateAsync(user);
            session.Clear();
        
            session.SetString($"Access_Token", token.AccessToken);
            session.SetString($"Refresh_Token", token.RefreshToken);
            
            return result;
           

        }

        public async Task<LoginUserCommandResponse> Handle(LoginWithUserNameCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                throw new AppException("user NotFound");
            if (!user.IsConfirm && user.Status == Infrastracture.Enums.UserState.Pendding)
                throw new AppException("Please confirm your email");
            var checkPass = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!checkPass)
                throw new AppException("wrong password");
            var ip = _http.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault()
              ?? _http.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var server = _redis.Multiplexer.GetServer(_redis.Multiplexer.GetEndPoints()[0]);
            var existingSessions = server.Keys(pattern: $"refresh_token:{user.Id}:*").ToList();

            if (existingSessions.Any())
                throw new AppException("User is logged in!");

            var token = await _tokenService.GenerateTokenPairAsync(user, ip);
       
            
            var key = $"refresh_token:{user.Id}:{token.RefreshToken}";
            var data = new
            {
                UserId = user.Id,
                RefreshToken = token.RefreshToken,
                Ip = ip,
                Expiration = token.RefreshTokenExpiry.ToString("O")
            };

            var json = JsonSerializer.Serialize(data);

            var ttl = token.RefreshTokenExpiry - DateTime.Now;
            await _redis.StringSetAsync(key, json, ttl);
            if(user.Logins.Count > 0)
            {
                user.Logins.OrderBy(c => c.CreationDateTime).Last().IsActive = false;
            }
            

            var login = new AppLogin()
            {
                IP = ip,
                UserId = user.Id,
                LoginProvider = request.UserAgent,
                IsActive = true,
                ProviderKey = user.Id.ToString(),
                CreationDateTime = DateTime.Now,
                IsLoggedIn = true,
                DisplayName = LoginHelper.GenerateDisplayName(request.UserAgent),

            };
            user.Logins.Add(login);
           

            var tokenPair = await _tokenService.CreateSessionAsync(user, ip);
            var session = _http.HttpContext.Session;
            session.SetString($"Access_Token", token.AccessToken);
            session.SetString($"Refresh_Token", token.RefreshToken);
            _http.HttpContext.Response.Cookies.Append(
                        "refresh_token",
                        tokenPair.RefreshToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,           // set false only if you run without https locally
                            SameSite = SameSiteMode.Lax,
                            Expires = tokenPair.RefreshTokenExpiry
                        });


            await _userManager.UpdateAsync(user);
            return new LoginUserCommandResponse { Token = token.AccessToken };

        }

        public async Task Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new AppException("user NotFoun");
            if (user.IsConfirm)
                throw new AppException("User already confirmed");
            var otp = new Random().Next(100000, 999999).ToString();


            var emailText = @"
                    <!DOCTYPE html>
                    <html lang=""en"">
                    <body style=""font-family: Arial, sans-serif; background:#f9f9f9; padding:20px;"">
                        <div style=""
                            max-width:520px; 
                            margin:auto; 
                            background:white; 
                            padding:30px; 
                            border-radius:12px; 
                            box-shadow:0 4px 10px rgba(0,0,0,0.08);
                        "">
                            <h2 style=""margin:0; color:#2c3e50; text-align:center;"">🔐 Email Verification</h2>

                            <p style=""color:#555; font-size:15px; margin-top:18px; line-height:1.6;"">
                                Hello,<br><br>
                                Use the verification code below to complete your authentication:
                            </p>

                            <div style=""text-align:center; margin:25px 0;"">
                                <span style=""
                                    display:inline-block; 
                                    font-size:32px; 
                                    letter-spacing:6px; 
                                    padding:14px 30px; 
                                    background:#eef2ff; 
                                    border-radius:8px; 
                                    color:#111; 
                                    font-weight:bold;
                                "">
                                    {{otp}}
                                </span>
                            </div>

                            <p style=""color:#666; font-size:14px; line-height:1.6;"">
                                This code is valid for <strong>2 minutes</strong>.<br>
                                Never share this code with anyone.
                            </p>

                            <hr style=""border:0; border-top:1px solid #eee; margin:25px 0;"">

                            <p style=""margin:0; font-size:13px; color:#999; text-align:center;"">
                                Thank you,<br>
                                <strong>Your Authentication Team</strong>
                            </p>
                        </div>
                    </body>
                    </html>";


            emailText = emailText.Replace("{{otp}}", otp);
            Console.WriteLine(otp);
            // Store in session
            var session = _http.HttpContext.Session;
            var identityToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            session.SetString($"EMAIL_TOKEN:{user.Email}", identityToken);
            session.SetString($"OTP:{user.Email}", otp);
            session.SetString($"OTP_EXPIRE:{user.Email}", DateTime.Now.AddMinutes(2).ToString());
        }

        //public async Task Handle(UploadImageProfileCommand request, CancellationToken cancellationToken)
        //{
        //    var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        //    if (user == null)
        //        throw new AppException("User NotFround");
        //    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        //    if (!Directory.Exists(uploadsFolder))
        //        Directory.CreateDirectory(uploadsFolder);

        //    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ProfileImage.FileName)}";
        //    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await request.ProfileImage.CopyToAsync(stream, cancellationToken);
        //    }

        //    // Store relative path in database
        //    user.Profile.ProfileImageUrl = $"/uploads/{uniqueFileName}";

        //    await _userManager.UpdateAsync(user);
        //}

        public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(request.AccessToken))
            {
                throw new AppException("there is no logged in user");
            }
            if (string.IsNullOrEmpty(request.RefreshToken))
                throw new AppException("refresh token not found");

            // 1. Validate access token to extract userId
            var validation = _tokenService.ValidateAccessToken(request.AccessToken);
            if (!validation.IsValid && string.IsNullOrEmpty(validation.UserId))
                throw new AppException("tokens are not valid");

            var userId = validation.UserId;
            // 2. Remove refresh token Redis entry
            var refreshKey = $"refresh_token:{userId}:{request.RefreshToken}";
            await _redis.KeyDeleteAsync(refreshKey);

            // 3. Remove session tracking
            var sessionKey = $"user:{userId}:sessions";
            await _redis.SetRemoveAsync(sessionKey, request.RefreshToken);

            // 4. Optional – blacklist access token until it expires
            var expiry = _tokenService.GetAccessTokenExpiry(request.AccessToken);
            if (expiry > DateTime.UtcNow)
            {
                var ttl = expiry - DateTime.UtcNow;
                await _redis.StringSetAsync(
                    $"blacklist:{request.AccessToken}",
                    "1",
                    ttl
                );
            }

        }

        public async Task Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new AppException("User NotFound");
            user.UserName = request.UserName;
            user.FullName = request.FullName;
            await _userManager.UpdateAsync(user);
        }
    }
}
