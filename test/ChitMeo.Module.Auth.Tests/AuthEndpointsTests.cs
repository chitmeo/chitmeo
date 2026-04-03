using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth;
using ChitMeo.Module.Auth.Api.Login;
using ChitMeo.Module.Auth.Api.User;
using ChitMeo.Module.Auth.Api.System;
using ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;
using ChitMeo.Module.Auth.Application.UseCases.Systems.Queries;
using ChitMeo.Module.Auth.Application.UseCases.Users.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace ChitMeo.Module.Auth.Tests;

public class AuthEndpointsTests
{
    private static HttpClient CreateTestClient(IMediator mediator)
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(mediator);
                services.AddRouting();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints => new AuthModule().MapEndpoints(endpoints));
            });

        var server = new TestServer(builder);
        return server.CreateClient();
    }

    [Fact]
    public async Task Ping_ReturnsOkWithMessage()
    {
        var mediator = Substitute.For<IMediator>();
        using var client = CreateTestClient(mediator);

        var response = await client.GetAsync("/auth/ping");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        payload.GetProperty("message").GetString().Should().Be("Auth module is working");
    }

    [Fact]
    public async Task Version_ReturnsConfiguredVersion()
    {
        var mediator = Substitute.For<IMediator>();
        mediator.SendAsync(Arg.Any<GetVersion.Command>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GetVersion.Result("Auth", "v1.0.0")));

        using var client = CreateTestClient(mediator);

        var response = await client.GetAsync("/auth/version");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<GetVersion.Result>();
        result.Should().NotBeNull();
        result!.Module.Should().Be("Auth");
        result.Version.Should().Be("v1.0.0");
    }

    [Fact]
    public async Task GetScript_ReturnsScriptText()
    {
        var mediator = Substitute.For<IMediator>();
        mediator.SendAsync(Arg.Any<GetScript.Request>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GetScript.Result("SELECT 1;")));

        using var client = CreateTestClient(mediator);

        var response = await client.GetAsync("/auth/script");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var script = await response.Content.ReadAsStringAsync();
        script.Should().Contain("SELECT 1;");
    }

    [Fact]
    public async Task LoginWithPassword_ReturnsAuthResponse()
    {
        var mediator = Substitute.For<IMediator>();
        mediator.SendAsync(Arg.Any<PasswordLogin.Command>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PasswordLogin.AuthResponse("access-token", "refresh-token")));

        using var client = CreateTestClient(mediator);

        var response = await client.PostAsJsonAsync("/auth/login/password", new { Email = "u@d.com", Password = "p" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PasswordLogin.AuthResponse>();
        payload.Should().NotBeNull();
        payload!.AccessToken.Should().Be("access-token");
        payload.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task GoogleLogin_ReturnsAuthResponse()
    {
        var mediator = Substitute.For<IMediator>();
        mediator.SendAsync(Arg.Any<GoogleLogin.Command>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GoogleLogin.AuthResponse("google-access-token")));

        using var client = CreateTestClient(mediator);

        var response = await client.PostAsJsonAsync("/auth/login/google", new { Token = "fake" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<GoogleLogin.AuthResponse>();
        payload.Should().NotBeNull();
        payload!.AccessToken.Should().Be("google-access-token");
    }

    [Fact]
    public async Task GetMe_ReturnsUserProfile()
    {
        var mediator = Substitute.For<IMediator>();
        var expected = new GetUserById.Response(Guid.NewGuid(), "x@x.com", "X User");
        mediator.SendAsync(Arg.Any<GetUserById.Query>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expected));

        using var client = CreateTestClient(mediator);

        var response = await client.GetAsync($"/auth/me?userId={expected.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<GetUserById.Response>();
        payload.Should().NotBeNull();
        payload!.Email.Should().Be("x@x.com");
    }

    [Theory]
    [InlineData("/auth/logout", "post")]
    [InlineData("/auth/logout-all", "post")]
    [InlineData("/auth/refresh-token", "get")]
    [InlineData("/auth/register", "post")]
    [InlineData("/auth/change-password", "post")]
    [InlineData("/auth/forgot-password", "get")]
    [InlineData("/auth/reset-password", "get")]
    [InlineData("/auth/set-password", "post")]
    [InlineData("/auth/link-google", "get")]
    [InlineData("/auth/unlink-google", "post")]
    public async Task NotImplementedEndpoints_ReturnInternalServerError(string route, string method)
    {
        var mediator = Substitute.For<IMediator>();
        using var client = CreateTestClient(mediator);

        Func<Task> call = method.ToLowerInvariant() switch
        {
            "get" => async () => await client.GetAsync(route),
            "post" => async () => await client.PostAsync(route, new StringContent(string.Empty)),
            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };

        await call.Should().ThrowAsync<NotImplementedException>();
    }
}
