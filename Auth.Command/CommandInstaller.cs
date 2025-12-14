using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class CommandInstaller
{
    public static void InstallCommands(this IServiceCollection services, Assembly assembly)
    {
        Assembly assembly2 = assembly;
        services.AddMediatR(delegate (MediatRServiceConfiguration opt)
        {
            opt.RegisterServicesFromAssembly(assembly2);
        });
    }
}


public static class AppCommandInstaller
{
    public static void InstallCommandsAndCommandValidators(this IServiceCollection services)
    {
        services.InstallCommands(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}