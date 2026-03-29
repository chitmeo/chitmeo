using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Unlink;

public class UnlinkGoogleEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/unlink-google",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("UnlinkGoogle")
            .WithTags("Unlink")
            .WithSummary("Unlink Google account")
            .WithDescription("Unlinks the user's account from their Google account")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
