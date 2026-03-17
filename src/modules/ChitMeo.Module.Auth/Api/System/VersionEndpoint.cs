using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Systems.Queries;
using ChitMeo.Shared.Abstractions.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.System;

public class VersionEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/version", Handle)
            .WithName("Version")
            .WithTags("Info");
    }

    private static async Task<IResult> Handle(IMediator mediator)
    {
        var result = await mediator.SendAsync(new GetVersion.Command());
        return Results.Ok(result);
    }
}
