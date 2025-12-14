using Auth.Repository.Context;
using System.Reflection;

namespace Auth.Api.Extentions.MinimalApi
{
    public static class MinimalApiConfiguration
    {
        public static void AddAllMinimalApisFromAssembly(this IServiceCollection services)
        {
            services.Scan(scan =>
                scan.FromAssembliesOf(typeof(Program))
                    //scan.FromCallingAssembly<IMinimalApi>()
                    .AddClasses(classes => classes.AssignableTo<IMinimalApi>())
                    .AsImplementedInterfaces()
                    //.WithSingletonLifetime());
                    .WithScopedLifetime());
        }
        public static void RegisterMinimalApisEndPoints(this WebApplication app)
        {
            app.MapGet("/Touch", () => "GATEWAY&AUTH IS ALIVE");

            using var scope = app.Services.CreateScope();
            ApplicationDbcontext context = scope.ServiceProvider.GetRequiredService<ApplicationDbcontext>();

            var minimalApis = scope.ServiceProvider.GetServices<IMinimalApi>();
            foreach (var minimalApi in minimalApis)
            {
                minimalApi.RegisterMinimalApi(app);
            }
        }
        

    }
}
