using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Endpoints;

public class GoogleEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/google", Handle)
            .WithName("Google")
            .WithTags("External");
    }

    private static IResult Handle()
    {
        throw new NotImplementedException();
    }
}
