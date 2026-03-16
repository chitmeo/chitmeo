using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Shared.Abstractions.Modules;

public interface IModule
{
    string Name { get; }
    string RoutePrefix { get; }
    void ConfigureServices(IServiceCollection services, IConfiguration config);

    void MapEndpoints(IEndpointRouteBuilder endpoints);
}