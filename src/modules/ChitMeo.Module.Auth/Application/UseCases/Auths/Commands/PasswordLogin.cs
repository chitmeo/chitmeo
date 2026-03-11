using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class PasswordLogin
{
    public record AuthResponse(string AccessToken, string RefreshToken);
    public sealed class Command : IRequest<AuthResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal class Handler : IRequestHandler<Command, AuthResponse>
    {
        private readonly IAuthDbContext _context;
        public Handler(IAuthDbContext context)
        {
            _context = context;
        }

        public Task<AuthResponse> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
