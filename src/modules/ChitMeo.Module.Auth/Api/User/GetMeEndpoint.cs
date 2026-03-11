using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Users.Queries;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.User;

public class GetMeEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/me", async (
                GetUserById.Query query,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.SendAsync(query, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetMe")
            .WithTags("User")
            .WithSummary("Get current user profile")
            .WithDescription("Returns current user profile")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
