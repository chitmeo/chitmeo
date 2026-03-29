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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            var password = await _context.UserPasswords.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
            if (password == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, password.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user);
            return new AuthResponse(accessToken, refreshToken);
        }

    }
}
