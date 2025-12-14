using Auth.Api.Extentions.MinimalApi;
using Auth.Command.Commands.AppProjects;
using Auth.Query.Queries.AppGroups;
using Auth.Query.Queries.AppProjects;
using MediatR;

namespace Auth.Api.MinimalApis.AppProjects
{
    public class AppProjectMinimalApi : IMinimalApi
    {
        public void RegisterMinimalApi(WebApplication app)
        {
            app.MapGet("/api/Projects/GetAllProjects/{pageNumber}/{pageSize}", GetAllProjects)
                .HasDescription("Get all registered projects");
            app.MapGet("/api/Projects/GetUrlsByProjectId", GetUrlsByProjectId)
                .HasDescription("Get all registered Url By project id");

            app.MapPost("/api/Projects/AddUrlToGroup", AddUrlToGroup)
                .HasDescription("Add Url Access to Group");
            app.MapPost("/api/Projects/DeleteUrlFromGroup", DeleteUrlFromGroup)
                .HasDescription("Delete Url Access From Group");
        }

        public async Task<IResult> GetAllProjects(IMediator mediator, int pageNumber, int pageSize)
        {
            var request = new GetAllProjectQuery() { PageNumber = pageNumber, PageSize = pageSize };
            var result = await mediator.Send(request);
            return Results.Ok(result);
        }
        public async Task<IResult> GetUrlsByProjectId(IMediator mediator,Guid ProjectId, int pageNumber, int pageSize,
            string? HttpMethod, string? Search, string? SortBy, bool SortDesc)
        {
            var filter = new AppUrlFilter { HttpMethod = HttpMethod, Search = Search, SortBy = SortBy, SortDesc = SortDesc };
            var request = new GetUrlsByProjectIdQuery() {ProjectId = ProjectId, PageNumber = pageNumber, PageSize = pageSize , UrlFilter = filter};
            var result = await mediator.Send(request);
            return Results.Ok(result);
        }

        public async Task<IResult> AddUrlToGroup(IMediator mediator, AddUrlToGroupCommand command)
        {
            await mediator.Send(command);
            return Results.Ok();    
        }
        public async Task<IResult> DeleteUrlFromGroup(IMediator mediator, RemoveUrlFromGroupCommand command)
        {
            await mediator.Send(command);
            return Results.Ok();
        }
    }
}
