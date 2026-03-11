using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Link;

public class LinkGoogleEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/link-google", async () =>
            {
                throw new NotImplementedException();
            })
            .WithName("LinkGoogle")
            .WithTags("Link")
            .WithSummary("Link Google account")
            .WithDescription("Links the user's account with their Google account")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
