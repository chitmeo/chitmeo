using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Tokens.Queries;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Token;

public class RefreshTokenEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/refresh-token", async (
            [FromBody] Refresh.Query request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.SendAsync(request, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("RefreshToken")
        .WithTags("Token")
        .WithSummary("Refresh authentication token")
        .WithDescription("Refreshes the authentication token using a refresh token")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();
    }
}