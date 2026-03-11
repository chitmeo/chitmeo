using System.Reflection;
using ChitMeo.Mediator;
using ChitMeo.Shared.Abstractions.Modules;

namespace ChitMeo.Host.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static void ConfigureWebApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddChitMeoModules(builder.Configuration);
    }

    public static IServiceCollection AddChitMeoModules(this IServiceCollection services, IConfiguration config)
    {
        var enabledModules = config
              .GetSection("Modules")
              .Get<Dictionary<string, bool>>() ?? new();


        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var assemblies = Array.Empty<Assembly>();
        if (!string.IsNullOrEmpty(path))
        {
            assemblies = Directory.GetFiles(path, "ChitMeo.Module.*.dll")
                          .Select(Assembly.LoadFrom)
                          .ToArray();
        }
        if (!assemblies.Any())
        {
            return services;
        }

        var modules = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .Select(t => (IModule)Activator.CreateInstance(t)!)
            .Where(m => enabledModules.GetValueOrDefault(m.Name, true))
            .ToList();

        foreach (var module in modules)
        {
            module.ConfigureServices(services);
        }

        if (modules.Any())
        {
            services.AddSingleton<IEnumerable<IModule>>(modules);
            services.AddMediator();
        }

        return services;
    }
}
