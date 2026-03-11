using System.Reflection;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Shared.Infrastructure.Endpoints;

public static class EndpointMapper
{
    public static void MapEndpointsFromAssembly(
        this RouteGroupBuilder group,
        Assembly assembly)
    {
        var endpointTypes = assembly.GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t)
                     && !t.IsInterface
                     && !t.IsAbstract);

        foreach (var type in endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(type)!;
            endpoint.Map(group);
        }
    }
}