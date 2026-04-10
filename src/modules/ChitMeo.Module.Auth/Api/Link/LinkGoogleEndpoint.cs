using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ChitMeo.Module.Auth.Api.Link;

public class LinkGoogleEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/google/link", async (
            [FromBody] LinkGoogleRequest request,
            HttpContext context,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new GoogleLink.Command
            {
                Token = request.Token,
                UserId = userId
            };

            var result = await mediator.SendAsync(command, cancellationToken);
            return result ? Results.Ok() : Results.BadRequest("Failed to link Google account");
        })
        .WithName("GoogleLink")
        .WithTags("Link")
        .WithSummary("Link Google account")
        .WithDescription("Links the user's account with their Google account")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
    }
}

public record LinkGoogleRequest(string Token);
