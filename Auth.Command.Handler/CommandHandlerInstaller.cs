using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Auth.Command.Handler;

public static class AppCommandHandlerInstaller
{
    public static void InstallCommandHandlers(this IServiceCollection services)
    {
        services.InstallCommandHandlers(Assembly.GetExecutingAssembly());
    }
}


public static class CommandHandlerInstaller
{
    public static void InstallCommandHandlers(this IServiceCollection services, Assembly assembly)
    {
        Assembly assembly2 = assembly;
        services.AddMediatR(delegate (MediatRServiceConfiguration opt)
        {
            opt.RegisterServicesFromAssembly(assembly2);
        });
    }

    public static Type[]? InstallCommandAutoMapperProfiles()
    {
        return (from item in Assembly.GetAssembly(typeof(Profile))?.GetTypes()
                 where item.IsClass && item.IsSubclassOf(typeof(Profile))
                 select item)?.ToArray();
    }
}