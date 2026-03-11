using ChitMeo.Shared.Abstractions.Modules;

namespace ChitMeo.Host.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task ConfigureRequestPipelineAsync(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseChitMeoModules();
        //run
        await app.RunAsync();
    }

    private static IApplicationBuilder UseChitMeoModules(this WebApplication app)
    {
        var modules = app.Services.GetRequiredService<IEnumerable<IModule>>();

        foreach (var module in modules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }
}
