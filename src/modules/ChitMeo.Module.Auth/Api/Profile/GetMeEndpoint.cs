using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Profile;

public class GetMeEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/me",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("GetMe")
            .WithTags("Profile")
            .WithSummary("Get current user profile")
            .WithDescription("Returns current user profile")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
