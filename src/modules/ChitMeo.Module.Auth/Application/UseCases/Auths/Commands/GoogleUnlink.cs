using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain;
using ChitMeo.Module.Auth.Domain.Entities;
using ChitMeo.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class GoogleUnlink
{
    public record Command : IRequest<bool>
    {
        [Required(ErrorMessage = "UserId is required.")]
        public Guid UserId { get; set; }
    }

    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IAuthDbContext _dbContext;

        public Handler(IAuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            ValidationHelper.ValidateAndThrow(request);
            var externalLogin = await ValidateAndThrowAsync(request, cancellationToken);

            if (externalLogin == null)
            {
                return false; // Not linked
            }

            _dbContext.ExternalLogins.Remove(externalLogin);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        private async Task<ExternalLogin?> ValidateAndThrowAsync(Command request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                throw new ValidationException("UserId is required.");
            }

            return await _dbContext.ExternalLogins.AsTracking()
                .FirstOrDefaultAsync(el => el.UserId == request.UserId && el.Provider == AuthProvider.Google.ToString(), cancellationToken);
        }
    }
}
