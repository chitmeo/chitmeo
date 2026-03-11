using ChitMeo.Mediator;
using ChitMeo.Module.Auth;
using ChitMeo.Shared.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Module.Blog;

[DependsOn(typeof(AuthModule))]
public class BlogModule : IModule
{
    public string Name => "Blog";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMediator();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapControllers();
    }
}
