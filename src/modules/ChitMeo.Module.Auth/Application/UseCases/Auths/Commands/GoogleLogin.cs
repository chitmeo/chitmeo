using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain.Entities;
using ChitMeo.Shared.Helpers;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class GoogleLogin
{
    public record AuthResponse(string AccessToken);
    public sealed class Command : IRequest<AuthResponse>
    {
        [Required(ErrorMessage = "Google token is required.")]
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
            ValidationHelper.ValidateAndThrow(request);
            var payload = await ValidateAsync(request, cancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = payload.Name,
                Email = payload.Email,
                EmailConfirmed = false
            };

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

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

        private async Task<GoogleJsonWebSignature.Payload> ValidateAsync(Command request, CancellationToken cancellationToken)
        {
            // Validate Google token and get user info
            var payload = null as GoogleJsonWebSignature.Payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);
            }
            catch (InvalidJwtException)
            {
                throw new InvalidOperationException("Invalid Google token.");
            }
            // Check if user with the same email already exists
            var existingUser = await _context.Users.FirstAsync(x => x.Email == payload.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {payload.Email} already exists.");
            }
            return payload;
        }
    }
}