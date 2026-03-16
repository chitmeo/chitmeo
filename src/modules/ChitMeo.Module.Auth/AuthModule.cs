using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Application.Configurations;
using ChitMeo.Module.Auth.Infrastructure.Security;
using ChitMeo.Shared.Abstractions.Modules;
using ChitMeo.Shared.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Module.Auth;

public class AuthModule : IModule
{
    public string Name => "Auth";
    public string RoutePrefix => "/auth";

    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection("Jwt"));
        services.Configure<GoogleOptions>(config.GetSection("Authentication:Google"));
        services.AddScoped<ITokenService, JwtTokenService>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup(RoutePrefix)
            .WithTags(Name);

        group.MapEndpointsFromAssembly(typeof(AuthModule).Assembly);
    }
}

