using System;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Authentication;

public class LoginEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/login", Handle)
            .WithName("Login")
            .WithTags("Authentication");
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
