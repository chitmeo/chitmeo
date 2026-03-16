using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Authentication;

public class GoogleLoginEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/google-login", async (string? returnUrl) =>
        {
            var redirectUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id=YOUR_CLIENT_ID&redirect_uri=YOUR_REDIRECT_URI&response_type=code&scope=email%20profile&state={Uri.EscapeDataString(returnUrl ?? string.Empty)}";
            return Results.Redirect(redirectUrl);
        }).WithName("GoogleLogin");
    }
}
