using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.Auth.Application.UseCases.Tokens.Queries;

public static class Refresh
{
    public record Response(string AccessToken, string RefreshToken);
    public sealed class Query : IRequest<Response>
    {
        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    internal class Handler : IRequestHandler<Query, Response>
    {
        private readonly IAuthDbContext _context;
        private readonly ITokenService _tokenService;

        public Handler(IAuthDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<Response> HandleAsync(Query request, CancellationToken cancellationToken)
        {

            var user = await ValidateAndThrowAsync(request, cancellationToken);

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = Guid.NewGuid().ToString(); // Or better random string
            var newHashedRefreshToken = _tokenService.HashToken(newRefreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newHashedRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIP = "" // Could get from context, but for now empty
            };

            await _context.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response(newAccessToken, newRefreshToken);
        }

        private async Task<User> ValidateAndThrowAsync(Query request, CancellationToken cancellationToken)
        {
            var hashedToken = _tokenService.HashToken(request.RefreshToken);

            var refreshTokenEntity = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == hashedToken && rt.ExpiresAt > DateTime.UtcNow, cancellationToken);

            if (refreshTokenEntity == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            var user = await _context.Users.FindAsync(refreshTokenEntity.UserId, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            return user;
        }
    }

}
