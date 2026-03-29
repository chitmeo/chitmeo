using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;
using ChitMeo.Shared.Abstractions.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Login;

public class LoginWithPasswordEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login/password", async (
                [FromBody] PasswordLogin.Command command,    
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.SendAsync(command, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("LoginWithPassword")
            .WithTags("Authentication")
            .WithSummary("Login with email and password")
            .WithDescription("Authenticate user using email and password")
            .AllowAnonymous();
    }
}
