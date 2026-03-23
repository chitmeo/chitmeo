using ChitMeo.Module.Auth.Application.Configurations;
using ChitMeo.Shared.Abstractions.Modules;
using ChitMeo.Shared.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ChitMeo.Module.Auth;

public class AuthModule : IModule
{
    public string Name => "Auth";
    public string RoutePrefix => "/auth";

    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        var jwtOptions = config.GetSection("Jwt").Get<JwtOptions>();
        services.Configure<JwtOptions>(config.GetSection("Jwt"));
        services.Configure<GoogleOptions>(config.GetSection("Authentication:Google"));

        services.AddDbContext(config);
        // Register services
        services.AddServices();

        if (jwtOptions != null)
        {
            ConfigureJwtAuthentication(services, jwtOptions);
        }
    }

    private static void ConfigureJwtAuthentication(IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.Key))
            };
        });
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup(RoutePrefix)
            .WithTags(Name);

        group.MapEndpointsFromAssembly(typeof(AuthModule).Assembly);
    }
}

