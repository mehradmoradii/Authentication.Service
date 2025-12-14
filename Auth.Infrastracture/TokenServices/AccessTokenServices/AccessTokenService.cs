
using Auth.Domain.AppUsers.Aggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastracture.TokenServices.AccessTokenServices
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public AccessTokenService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateAccessToken(AppUser user)
        {
            
            var claims = new List<Claim>()
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddMinutes(_configuration.GetValue<int>("AuthSetting:AccessTokenExpiryMinutes")).ToString()),
                new Claim("Is_confirmed", user.IsConfirm.ToString().ToLower()),

            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSetting:secretKey"]));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSetting:validIssuer"],
                audience: _configuration["AuthSetting:validAudience"],
                claims: claims,
                signingCredentials: cred,
                expires: DateTime.Now.AddMinutes(_configuration.GetValue<int>
                ("AuthSetting:AccessTokenExpiryMinutes")));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string? GetEmailFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var readedToken = tokenHandler.ReadJwtToken(token);
            var email = readedToken.Claims.FirstOrDefault(o=>o.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email);
            return email.Value;
        }

        public DateTime GetExpirationFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var readedToken = tokenHandler.ReadJwtToken(token);
            var expirationTime = readedToken.Claims.FirstOrDefault(t=>t.Type == ClaimTypes.Expiration);
            var convertedDateTime = DateTime.Parse(expirationTime.Value);
            return convertedDateTime;

        }

        public List<string> GetRolesFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var readedToken = tokenHandler.ReadJwtToken(token);
            var roles = readedToken.Claims.Where(t=>t.Type == ClaimTypes.Role).Select(v=>v.Value).ToList();
            return roles;
        }

        public string? GetUserIdFromToken(string token)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var readedToken = tokenhandler.ReadJwtToken(token);
            var userId = readedToken.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return userId;
        }

        public TokenValidationResponse ValidateAccessToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["AuthSetting:secretKey"]);

                var tokenParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["AuthSetting:validIssuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["AuthSetting:validAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, tokenParams, out SecurityToken validatedToken);

                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return new TokenValidationResponse { IsValid = false, Error = "UserId claim missing" };
                }

                return new TokenValidationResponse
                {
                    IsValid = true,
                    UserId = userIdClaim.Value
                };
            }
            catch (Exception ex)
            {
                return new TokenValidationResponse
                {
                    IsValid = false,
                    Error = ex.Message
                };
            }
        }

    }
}
