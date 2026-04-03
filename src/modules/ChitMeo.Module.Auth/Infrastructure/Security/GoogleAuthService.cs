using ChitMeo.Module.Auth.Application.Abstractions;
using Google.Apis.Auth;

namespace ChitMeo.Module.Auth.Infrastructure.Security;

public class GoogleAuthService : IGoogleAuthService
{
    public Task<GoogleJsonWebSignature.Payload> ValidateAsync(string token)
    {
        return GoogleJsonWebSignature.ValidateAsync(token);
    }
}