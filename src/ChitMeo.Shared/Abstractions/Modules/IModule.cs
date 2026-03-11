using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Shared.Abstractions.Modules;

public interface IModule
{
    string Name { get; }
    string RoutePrefix { get; }
    void ConfigureServices(IServiceCollection services);

    void MapEndpoints(IEndpointRouteBuilder endpoints);
}