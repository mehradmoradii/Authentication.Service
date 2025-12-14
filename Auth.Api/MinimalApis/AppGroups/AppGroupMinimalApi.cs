using Auth.Api.Extentions.MinimalApi;
using Auth.Command.Commands.AppGroups;
using Auth.Query.Queries.AppGroups;
using MediatR;

namespace Auth.Api.MinimalApis.AppGroups
{
    public class AppGroupMinimalApi : IMinimalApi
    {
        public void RegisterMinimalApi(WebApplication app)
        {
            app.MapGet("/api/Groups/GetAll/{pageNumber}/{pageSize}", GetAllAppGroups).HasDescription("Get All Groups");
            app.MapPost("/api/Groups/Create", CreateAppGroups).HasDescription("Create Group");
            app.MapPost("/api/Groups/AssignUser", AssignUserToGroup).HasDescription("Add User to a group");
                
        }

        public async Task<IResult> GetAllAppGroups(IMediator mediator, int pageNumber, int pageSize)
        {
            var request = new GetAllAppGroupQuery { pageNumber = pageNumber, pageSize = pageSize };
            var result = await mediator.Send(request);
            return Results.Ok(result);
        }

        public async Task<IResult> CreateAppGroups(IMediator mediator,CreateAppGroupCommand command)
        {
            await mediator.Send(command);
            return Results.Ok();
        }
        public async Task<IResult> AssignUserToGroup(IMediator mediator, AssignGroupToUserCommand command)
        {
            await mediator.Send(command);
            return Results.Ok();
        }
    }
}
