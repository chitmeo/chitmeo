using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Token;

public class RefreshTokenEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/refresh-token",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("RefreshToken")
            .WithTags("Token")
            .WithSummary("Refresh authentication token")
            .WithDescription("Refreshes the authentication token using a refresh token")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
