using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain.Entities;
using ChitMeo.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class Logout
{
    public sealed class Command : IRequest<bool>
    {
        [Required(ErrorMessage = "UserId is required.")]
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
            ValidationHelper.ValidateAndThrow(request);
            var refreshTokens = await ValidateAndThrowAsync(request, cancellationToken);

            if (!refreshTokens.Any())
            {
                return false; // No active sessions
            }

            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task<List<RefreshToken>> ValidateAndThrowAsync(Command request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                throw new ValidationException("UserId is required.");
            }

            return await _context.RefreshTokens
                .Where(rt => rt.UserId == request.UserId)
                .AsTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
