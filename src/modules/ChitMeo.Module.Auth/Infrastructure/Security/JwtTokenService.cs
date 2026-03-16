using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain.Entities;

namespace ChitMeo.Module.Auth.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    public string GenerateAccessToken(User user)
    {
        throw new NotImplementedException();
    }
}
