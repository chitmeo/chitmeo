using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Shared.Abstractions.Endpoints;

public interface IEndpoint
{
    void Map(RouteGroupBuilder group);
}