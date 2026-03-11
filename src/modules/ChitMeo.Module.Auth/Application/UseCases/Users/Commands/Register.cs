using System;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;

namespace ChitMeo.Module.Auth.Application.UseCases.Users.Commands;

public static class Register {

    public sealed class Command : IRequest<Guid> {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
    
    internal class Handler : IRequestHandler<Command, Response> {
        private readonly IAuthDbContext _context;
        public Handler(IAuthDbContext context) {
            _context = context;
        }
        public async Task<Response> HandleAsync(Command request, CancellationToken cancellationToken) {
            var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
            if (user == null) {
                throw new Exception("User not found");
            }
            return new Response(user.Id, user.Email, user.Name);
        }
    }
}