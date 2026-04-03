using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class Logout
{
    public sealed class Command : IRequest<bool>
    {
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
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == request.UserId)
                .AsTracking()
                .ToListAsync(cancellationToken);

            if (!refreshTokens.Any())
            {
                return false; // No active sessions
            }

            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
