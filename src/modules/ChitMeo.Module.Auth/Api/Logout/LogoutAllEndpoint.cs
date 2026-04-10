using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ChitMeo.Module.Auth.Api.Logout;

public class LogoutAllEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/logout-all", async (
            HttpContext context,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new LogoutAll.Command { UserId = userId };
            var result = await mediator.SendAsync(command, cancellationToken);
            return result ? Results.Ok() : Results.BadRequest("Failed to logout from all devices");
        })
        .WithName("LogoutAll")
        .WithTags("Authentication")
        .WithSummary("Logout from all devices")
        .WithDescription("Logs out the current user from all devices and invalidates all authentication tokens")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
    }
}
