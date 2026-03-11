using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Logout;

public class LogoutAllEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/logout-all",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("LogoutAll")
            .WithTags("Authentication")
            .WithSummary("Logout from all devices")
            .WithDescription("Logs out the current user from all devices and invalidates all authentication tokens")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
