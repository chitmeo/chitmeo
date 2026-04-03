using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
            // Validate: No duplicate email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            // If valid, insert into User and UserPassword
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                EmailConfirmed = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user, cancellationToken);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var userPassword = new UserPassword
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                PasswordHash = passwordHash,
                PasswordSalt = "", // BCrypt includes salt in hash
                IsActive = true
            };

            await _context.UserPasswords.AddAsync(userPassword, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}