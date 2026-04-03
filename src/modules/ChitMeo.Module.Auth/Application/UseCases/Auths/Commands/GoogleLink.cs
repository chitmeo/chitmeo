using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
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

        public Handler(IAuthDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);

            var user = await _context.Users.FindAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return false;
            }

            var existingLogin = await _context.ExternalLogins
                .FirstOrDefaultAsync(el => el.UserId == request.UserId && el.Provider == "Google", cancellationToken);

            if (existingLogin != null)
            {
                return true; // Already linked
            }

            var externalLogin = new ExternalLogin
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Provider = "Google",
                ProviderUserId = payload.Subject,
                Email = payload.Email,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ExternalLogins.AddAsync(externalLogin, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
