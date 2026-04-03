using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using Microsoft.EntityFrameworkCore;


namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class GoogleUnlink
{
    public record Command : IRequest<bool>
    {
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
            var externalLogin = await _dbContext.ExternalLogins.AsTracking()
                .FirstOrDefaultAsync(el => el.UserId == request.UserId && el.Provider == "Google", cancellationToken);

            if (externalLogin == null)
            {
                return false; // Not linked
            }

            _dbContext.ExternalLogins.Remove(externalLogin);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
    
    

}
