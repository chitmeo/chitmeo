using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.System;

public class VersionEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/version", Handle)
            .WithName("Version")
            .WithTags("Info");
    }

    private static IResult Handle()
    {
        return Results.Ok(new
        {
            module = "Auth",
            version = "1.0.0"
        });
    }
}
