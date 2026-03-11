using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Logout;

public class LogoutEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/logout",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("Logout")
            .WithTags("Authentication")
            .WithSummary("Logout current user")
            .WithDescription("Logs out the current user and invalidates the authentication token")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
