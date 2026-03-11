using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.User;

public class SetPasswordEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/set-password",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("SetPassword")
            .WithTags("User")
            .WithSummary("Set user password")
            .WithDescription("Sets a new password for the current user")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
