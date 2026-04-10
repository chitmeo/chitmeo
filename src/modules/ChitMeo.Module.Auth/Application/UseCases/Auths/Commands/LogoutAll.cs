using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;


namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class LogoutAll
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

            await ValidateAndThrowAsync(request, cancellationToken);
            return true;
        }

        private async Task ValidateAndThrowAsync(Command request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                throw new ValidationException("UserId is required.");
            }

            var refreshTokens = _context.RefreshTokens.Where(rt => rt.UserId == request.UserId);
            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
