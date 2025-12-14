using Auth.Api.Extentions.MinimalApi;
using Auth.Api.Extentions.Services.EmailSenderService;
using Auth.Command.Commands.AppUsers;
using Auth.Infrastracture.ErrorHandler;
using Auth.Infrastracture.TokenServices;
using Auth.Query.Queries.AppUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Auth.Api.MinimalApis.AppUsers
{
    public class AppUserMinimalApi : IMinimalApi
    {
        private readonly IEmailSenderService _emailSender;
        private readonly ITokenService _tokenService;
        private readonly IWebHostEnvironment _env;
        public AppUserMinimalApi(IEmailSenderService emailSender, ITokenService tokenService, IWebHostEnvironment env)
        {
            _emailSender = emailSender;
            _tokenService = tokenService;
            _env = env;
        }
        public void RegisterMinimalApi(WebApplication app)
        {
            app.MapGet("/api/Users/GetByGroupId/{GroupId}", GetAppUserByGroupId).HasDescription("Get Groups's Users");
            app.MapPost("/api/Users/Register", RegisterAppUser).HasDescription("Register User");
            app.MapPost("/api/Users/EmailConfirmation", EmailConfirmation).HasDescription("Confirm User's Email");
            app.MapPost("/api/Users/ResendEmailConfirmationCode", ResendEmailConfirmationCode);
                        
            app.MapPost("/api/Users/LoginWithUsername", LoginWithUsername).HasDescription("User Login With Username").RequireAuthorization("TriggerAuth");
            app.MapPost("/api/Users/LogoutUser", LogoutUser).HasDescription("User log out");
            app.MapPost("/api/Users/UpdateUserData", UpdateUserData).HasDescription("User update data");

            //app.MapPost("/api/Users/UploadImageProfile", UploadImageProfile).DisableAntiforgery();
          
        }

        public async Task<IResult> GetAppUserByGroupId(IMediator mediator, Guid GroupId)
        {
            var request = new GetUsersByGroupIdQuery { GroupId = GroupId };
            var result = await mediator.Send(request);
            return Results.Ok(result);
        }
  
        public async Task<IResult> RegisterAppUser(IMediator mediator,HttpContext context ,RegisterAppUserCommand command)
        {
            var result = await mediator.Send(command);
            try
            {
                await _emailSender.SendEmail(result.Email, "User Confirmation", result.Message);
                foreach (var key in context.Session.Keys)
                {
                    Console.WriteLine($"{key} = {context.Session.GetString(key)}");
                }
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }
            
           
        }
       
        public async Task<IResult> EmailConfirmation(IMediator mediator,HttpContext context ,EmailConfirmationCommand command)
        {
            command.UserAgent = context.Request.Headers["User-Agent"];
            var result = await mediator.Send(command);
            foreach (var key in context.Response.Headers.Keys)
            {
                Console.WriteLine($"{context.Response.Headers[key]}");
            }

            foreach (var key in context.Session.Keys)
            {
                Console.WriteLine($"{key} = {context.Session.GetString(key)}");
            }
            return Results.Ok(result);
        }
        public async Task<IResult> ResendEmailConfirmationCode(IMediator mediator,HttpContext context ,ResendEmailConfirmationCommand command)
        {
            await mediator.Send(command);
            Console.WriteLine(context.Response.Headers.Keys);
            foreach (var key in context.Response.Headers.Keys)
            {
                Console.WriteLine($"{context.Response.Headers[key]}");
            }

            foreach (var key in context.Session.Keys)
            {
                Console.WriteLine($"{key} = {context.Session.GetString(key)}");
            }
            return Results.Ok();
        }


        public async Task<IResult> LoginWithUsername(IMediator mediator,HttpContext context ,LoginWithUserNameCommand command)
        {
            if (context.User.Identity?.IsAuthenticated == true)
                throw new AppException("You are logged in!!!");

            command.UserAgent = context.Request.Headers["User-Agent"];
            var result = await mediator.Send(command);
            Console.WriteLine(context.Request.Headers);
            return Results.Ok(result);
        }
        
        public async Task<IResult> LogoutUser(IMediator mediator,HttpContext context )
        {
            var command = new LogoutUserCommand();
            command.AccessToken = context.Request.Headers["Authorization"]
         .FirstOrDefault()?.Replace("Bearer ", "");

            command.RefreshToken = context.Request.Cookies["refresh_token"]
                ?? context.Request.Headers["X-Refresh-Token"].FirstOrDefault();
            
            command.IpAddress = context.Connection.RemoteIpAddress?.ToString();

            await mediator.Send(command);

            // Remove refresh token cookie
            context.Response.Cookies.Delete("refresh_token");

            return Results.Ok();
        }
        
        public async Task<IResult> UpdateUserData(IMediator mediator,HttpContext context ,UpdateAppUserCommand command)
        {
            var AccessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
            if (AccessToken == null)
                throw new AppException("Invalid Token");
            var userId = _tokenService.GetUserIdFromAccessToken(AccessToken);
            command.UserId = Guid.Parse(userId);
            await mediator.Send(command);
            return Results.Ok();
        }
        //[IgnoreAntiforgeryToken]
        //public async Task<IResult> UploadImageProfile(IMediator mediator,HttpContext context, [FromForm] IFormFile file )
        //{
            
        //    if (file == null)
        //        throw new AppException("No file uploaded");

        //    var folderPath = Path.Combine(_env.WebRootPath, "uploads");
        //    var folderExists = Directory.Exists(folderPath);
        //    if(!folderExists)
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }
            

        //    var filePath = Path.Combine(folderPath, file.FileName);

        //    using var stream = new FileStream(filePath, FileMode.Create);
        //    await file.CopyToAsync(stream);

        //    return Results.Ok("Uploaded");
        //}

        
    }
}
