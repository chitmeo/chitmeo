using Google.Apis.Auth;

namespace ChitMeo.Module.Auth.Application.Abstractions;

public interface IGoogleAuthService
{
    Task<GoogleJsonWebSignature.Payload> ValidateAsync(string token);
}