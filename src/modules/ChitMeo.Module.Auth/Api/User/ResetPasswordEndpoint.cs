using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.User;

public class ResetPasswordEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/reset-password",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("ResetPassword")
            .WithTags("User")
            .WithSummary("Reset user password")
            .WithDescription("Resets the user's password using a token")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
