using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ChitMeo.Module.Auth.Api.Logout;

public class LogoutEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/logout", async (
            HttpContext context,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new global::ChitMeo.Module.Auth.Application.UseCases.Auths.Commands.Logout.Command { UserId = userId };
            var result = await mediator.SendAsync(command, cancellationToken);
            return result ? Results.Ok() : Results.BadRequest("Failed to logout");
        })
        .WithName("Logout")
        .WithTags("Authentication")
        .WithSummary("Logout current user")
        .WithDescription("Logs out the current user and invalidates the authentication token")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
    }
}
