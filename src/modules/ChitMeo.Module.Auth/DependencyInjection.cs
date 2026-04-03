using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Infrastructure.Persistence;
using ChitMeo.Module.Auth.Infrastructure.Security;
using ChitMeo.Shared.Configuration.Options;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Module.Auth;

public static class DependencyInjection
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
    }

    public static void AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        // Resolve DataConnectionOptions from configuration
        var dataConnectionOptions = config.GetSection("DataConnection").Get<DataConnectionOptions>();
        if (dataConnectionOptions is null)
        {
            throw new InvalidOperationException("DataConnection section is not configured");
        }

        var provider = dataConnectionOptions.Provider;
        var connectionString = dataConnectionOptions.ConnectionString;

        switch (provider)
        {
            case DataProvider.MariaDB:
            case DataProvider.MySQL:
                services.AddScoped<IAuthDbContext, AuthDbContext>(sp =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
                    optionsBuilder.UseMySQL(connectionString);
                    return new AuthDbContext(optionsBuilder.Options);
                });
                break;
            case DataProvider.SqlServer:
                services.AddScoped<IAuthDbContext, AuthDbContext>(sp =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
                    optionsBuilder.UseSqlServer(connectionString);
                    return new AuthDbContext(optionsBuilder.Options);
                });
                break;

            default:
                throw new InvalidOperationException("Unsupported database provider");
        }
    }
}

