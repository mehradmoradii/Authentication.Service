using System.Reflection;
using Microsoft.Extensions.DependencyInjection;


namespace Auth.Query.Handler;

public static class QueryHandlerInstaller
{
    public static void InstallQueryHandlers(this IServiceCollection services, Assembly assembly)
    {
        Assembly assembly2 = assembly;
        services.AddMediatR(delegate (MediatRServiceConfiguration opt)
        {
            opt.RegisterServicesFromAssembly(assembly2);
        });
    }
}

public static class AppQueryHandlerInstaller
{
    public static void InstallQueryHandlers(this IServiceCollection services)
    {
        services.InstallQueryHandlers(Assembly.GetExecutingAssembly());
    }
}