using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Systems.Queries;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.System;

public class GetScriptEndpoint : IEndpoint
{

    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/script",
            async (IMediator mediator,
                   CancellationToken cancellationToken) =>
            {
                var result = await mediator.SendAsync(new GetScript.Request(), cancellationToken);
                return Results.Ok(result.Script);
            })
            .WithName("GetScript")
            .WithTags("Info")
            .WithSummary("Get API script")
            .WithDescription("Returns current script of the API")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
