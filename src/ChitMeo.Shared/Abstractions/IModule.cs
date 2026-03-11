using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Shared.Abstractions;

public interface IModule
{
    string Name { get; }
    void ConfigureServices(IServiceCollection services);

    void MapEndpoints(IEndpointRouteBuilder endpoints);
}