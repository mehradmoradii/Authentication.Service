using Auth.Api.Extentions.MinimalApi;
using Auth.Command.Commands.AppRoles;
using MediatR;

namespace Auth.Api.MinimalApis.AppRoles
{
    public class AppRoleMinimalApi : IMinimalApi
    {
        public void RegisterMinimalApi(WebApplication app)
        {
            app.MapPost("/api/Roles/Create", CreateRole).HasDescription("Create Role");
            app.MapPost("/api/Roles/AssignUser", AssignRoleToUser).HasDescription("Add role to user");
        }

        public async Task<IResult> CreateRole(IMediator mediator, CreateAppRoleCommand command)
        {
            await mediator.Send(command);
            return Results.Ok();
        }
        public async Task<IResult> AssignRoleToUser(IMediator mediator , AssignRoleToUserCommand command)
        {
            await mediator.Send(command);
            return Results.Ok();
        }
    }
}
