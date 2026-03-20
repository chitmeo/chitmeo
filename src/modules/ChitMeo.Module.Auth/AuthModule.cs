using ChitMeo.Module.Auth.Application.Configurations;
using ChitMeo.Shared.Abstractions.Modules;
using ChitMeo.Shared.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        services.AddDbContext(config);
        // Register services
        services.AddServices();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidAudience = config["Jwt:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"]))
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

