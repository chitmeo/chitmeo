using ChitMeo.Mediator;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class LogoutAll
{
    public sealed class Command : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }

    internal class Handler : IRequestHandler<Command, bool>
    {
        public async Task<bool> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
