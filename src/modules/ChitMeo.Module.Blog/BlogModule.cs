using ChitMeo.Module.Auth;
using ChitMeo.Shared.Abstractions.Modules;
using ChitMeo.Shared.Infrastructure.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Module.Blog;

[DependsOn(typeof(AuthModule))]
public class BlogModule : IModule
{
    public string Name => "Blog";
    public string RoutePrefix => "/blog";

    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {

    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup(RoutePrefix)
            .WithTags(Name);

        group.MapEndpointsFromAssembly(typeof(BlogModule).Assembly);
    }
}
