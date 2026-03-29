using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;

namespace ChitMeo.Module.Auth.Application.UseCases.Users.Commands;

public static class Register
{

    public sealed class Command : IRequest<Guid>
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Name { get; set; } = default!;
    }

    internal class Handler : IRequestHandler<Command, Guid>
    {
        private readonly IAuthDbContext _context;
        public Handler(IAuthDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            //return 
            return new Guid();
        }
    }
}