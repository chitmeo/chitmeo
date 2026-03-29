using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;


namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class GoogleUnlink
{
    public record Command : IRequest<bool>
    {
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
            throw new NotImplementedException();
        }
    }
    
    

}
