using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;
using ChitMeo.Shared.Abstractions.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Authentication;

public class GoogleLoginEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/google", async (
            GoogleLogin.Command command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.SendAsync(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GoogleLogin")
        .WithSummary("Login with Google")
        .WithDescription("Authenticate user using Google ID Token")
        .AllowAnonymous();
    }
}
