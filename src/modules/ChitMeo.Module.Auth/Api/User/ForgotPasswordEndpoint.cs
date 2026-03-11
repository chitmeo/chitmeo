using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.User;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/forgot-password",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("ForgotPassword")
            .WithTags("User")
            .WithSummary("Initiate password reset process")
            .WithDescription("Initiates the password reset process for the user")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
