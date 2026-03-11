using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Endpoints.System;

public class PingEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/ping", Handle)
            .WithName("Ping")
            .WithTags("Health");
    }

    private static IResult Handle()
    {
        return Results.Ok(new
        {
            message = "Auth module is working",
            time = DateTime.UtcNow
        });
    }
}
