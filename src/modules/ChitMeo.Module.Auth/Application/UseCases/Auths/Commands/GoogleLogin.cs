using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Application.Configurations;
using ChitMeo.Module.Auth.Domain.Entities;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class GoogleLogin
{
    public record AuthResponse(string AccessToken);
    public sealed class Command : IRequest<AuthResponse>
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }

    internal class Handler : IRequestHandler<Command, AuthResponse>
    {
        private readonly IAuthDbContext _context;
        private readonly ITokenService _tokenService;

        public Handler(
            IAuthDbContext context,
            ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == payload.Email, cancellationToken);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = payload.Name,
                    Email = payload.Email,
                    EmailConfirmed = false
                };

                await _context.Users.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            //Hash access token and save to RefreshTokens table
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = _tokenService.HashToken(accessToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return new AuthResponse(accessToken);
        }
    }
}