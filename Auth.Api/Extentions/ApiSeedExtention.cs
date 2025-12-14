using Auth.Api.Extentions.MinimalApi;
using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppGroups.Entity;
using Auth.Domain.AppProjects.Aggregate;
using Auth.Domain.AppProjects.Entity;
using Auth.Domain.AppUsers.Aggregate;
using Auth.Repository.Context;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Extentions
{
    public class DiscoveredUrlDto
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; } = string.Empty;

    }

    public static class ApiSeedExtention
    {
        public static async Task CreateUrlsForProjectAsync(
                             this IServiceProvider services,
                             string projectTitle,
                             string host,
                             string port,
                             List<DiscoveredUrlDto> urls)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbcontext>();

            // 1️⃣ Load project or create it
            var project = await db.Set<AppProject>()
                .FirstOrDefaultAsync(p => p.Title == projectTitle);
            var adminGroups = await db.Set<AppGroup>().FirstOrDefaultAsync(i=>i.Title == "MainAdmin");
            if (project == null)
            {
                project = new AppProject
                {
                    Title = projectTitle,
                    Description = $"{projectTitle} Microservice",
                    Host = host,
                    Port = port
                };

                db.Set<AppProject>().Add(project);
                await db.SaveChangesAsync();
            }

            var newUrls = new List<AppUrl>();

            // 2️⃣ Loop through URLs provided by RabbitMQ
            foreach (var u in urls)
            {
                bool exists = await db.Set<AppUrl>()
                    .AnyAsync(x => x.Url == u.Url
                                && x.Method == u.Method
                                && x.ProjectId == project.Id);

                if (!exists)
                {
                    
                    
                    var segments = u.Url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    var filtered = segments.Where(s => !s.StartsWith("{") && !s.EndsWith("}"));
                    var newUrlText =  "/" + string.Join("/", filtered);
                    
                    var newUrl = new AppUrl
                    {
                        Title = $"{u.Method} {u.Url}",
                        Url =  newUrlText.ToLower(),
                        Method = u.Method,
                        Description = u.Description,
                        ProjectId = project.Id,
                        Groups = new List<AppGroupUrl>()
                    };
                    newUrls.Add(newUrl);
                    var perm = new AppGroupUrl { GroupId = adminGroups.Id, UrlId = newUrl.Id };
                    newUrl.Groups.Add(perm);
                }
            }

            // 3️⃣ Save new URLs
            if (newUrls.Any())
            {
                db.Set<AppUrl>().AddRange(newUrls);
                await db.SaveChangesAsync();
            }
        }


        public static List<DiscoveredUrlDto> DiscoverCurrentEndpoints(WebApplication app)
        {
            var endpointSource = app.Services.GetRequiredService<EndpointDataSource>();
            var urls = new List<DiscoveredUrlDto>();

            foreach (var ep in endpointSource.Endpoints.OfType<RouteEndpoint>())
            {
                var url = ep.RoutePattern.RawText;
                if (string.IsNullOrWhiteSpace(url)) continue;
                if (url.StartsWith("/swagger") || url.StartsWith("/_framework")) continue;

                var methods = ep.Metadata.OfType<HttpMethodMetadata>()
                    .FirstOrDefault()?.HttpMethods ?? new[] { "ANY" };

                var metadata = ep.Metadata.OfType<EndpointMetadataResponse>().FirstOrDefault();
                string description = metadata?.Description ?? "No description";
              

                foreach (var method in methods)
                {
                    urls.Add(new DiscoveredUrlDto
                    {
                        Url = url,
                        Method = method,
                        DisplayName = ep.DisplayName,
                        Description = description,
                        
                    });
                }
            }

            return urls;
        }

    }
}
