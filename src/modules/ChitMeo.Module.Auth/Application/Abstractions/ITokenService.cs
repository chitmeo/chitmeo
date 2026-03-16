using ChitMeo.Module.Auth.Domain.Entities;

namespace ChitMeo.Module.Auth.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(User user);

}
