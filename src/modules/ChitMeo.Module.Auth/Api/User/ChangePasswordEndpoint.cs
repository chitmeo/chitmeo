using System;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.User;

public class ChangePasswordEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/change-password",
            () =>
            {
                throw new NotImplementedException();
            })
            .WithName("ChangePassword")
            .WithTags("User")
            .WithSummary("Change current user password")
            .WithDescription("Changes current user password")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
