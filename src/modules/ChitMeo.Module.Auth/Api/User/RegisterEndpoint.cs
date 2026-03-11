using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.User;

public class RegisterEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/register",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("Register")
            .WithTags("User")
            .WithSummary("Register new user")
            .WithDescription("Registers a new user with email and password")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
