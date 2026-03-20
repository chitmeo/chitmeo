using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Application.Configurations;
using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChitMeo.Module.Auth.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _jwtOptions = options.Value;
    }
    public string GenerateAccessToken(User user)
    {
         var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_jwtOptions.ExpireDays),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
