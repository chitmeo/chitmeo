using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain;
using ChitMeo.Module.Auth.Domain.Entities;

using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class GoogleLink
{

    public sealed class Command : IRequest<bool>
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }
    }

    internal class Handler : IRequestHandler<Command, bool>
    {
        private readonly IAuthDbContext _context;
        private readonly IGoogleAuthService _googleAuthService;

        public Handler(IAuthDbContext context, IGoogleAuthService googleAuthService)
        {
            _context = context;
            _googleAuthService = googleAuthService;
        }

        public async Task<bool> HandleAsync(Command request, CancellationToken cancellationToken)
        {

            await ValidateAndThrow(request, cancellationToken);
            GoogleJsonWebSignature.Payload? payload;
            try
            {
                payload = await _googleAuthService.ValidateAsync(request.Token);
            }
            catch (InvalidJwtException)
            {
                throw new InvalidOperationException("Invalid Google token.");
            }

            var externalLogin = new ExternalLogin
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Provider = AuthProvider.Google.ToString(),
                ProviderUserId = payload.Subject,
                Email = payload.Email,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ExternalLogins.AddAsync(externalLogin, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private async Task ValidateAndThrow(Command request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {request.UserId} does not exist.");
            }

            var existingLogin = await _context.ExternalLogins
                .FirstOrDefaultAsync(el => el.UserId == request.UserId && el.Provider == AuthProvider.Google.ToString(), cancellationToken);

            if (existingLogin != null)
            {
                throw new InvalidOperationException($"User with ID {request.UserId} already has a linked Google account.");
            }
        }
    }

}
