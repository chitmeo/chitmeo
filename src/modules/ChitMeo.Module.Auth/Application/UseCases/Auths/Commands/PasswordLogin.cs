using BCrypt.Net;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class PasswordLogin
{
    public record AuthResponse(string AccessToken, string RefreshToken);
    public sealed class Command : IRequest<AuthResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal class Handler : IRequestHandler<Command, AuthResponse>
    {
        private readonly IAuthDbContext _context;
        private readonly ITokenService _tokenService;

        public Handler(IAuthDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            // Step 1: Check exist User by Email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Step 2: If exist check latest UserPassword to get current hash password in database
            var userPassword = await _context.UserPasswords
                .Where(up => up.UserId == user.Id && up.IsActive)
                .OrderByDescending(up => up.Id) // Assuming latest by ID, or add CreatedAt
                .FirstOrDefaultAsync(cancellationToken);

            if (userPassword == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Step 3: Hash Command.Password to compare
            // Assuming BCrypt is used
            if (!BCrypt.Net.BCrypt.Verify(request.Password, userPassword.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = Guid.NewGuid().ToString();
            var hashedRefreshToken = _tokenService.HashToken(refreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = hashedRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIP = "" // Could get from context
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResponse(accessToken, refreshToken);
        }

        private async Task<User> ValidateAndThrowAsync(Command request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return user;
        }
    }
}
