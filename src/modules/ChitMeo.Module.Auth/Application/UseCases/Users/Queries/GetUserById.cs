using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;

namespace ChitMeo.Module.Auth.Application.UseCases.Users.Queries;

public static class GetUserById
{
    public record Response(Guid Id, string Email, string Name);
    public sealed class Query : IRequest<Response>
    {
        public Guid UserId { get; set; }
    }

    internal class Handler : IRequestHandler<Query, Response>
    {
        private readonly IAuthDbContext _context;
        public Handler(IAuthDbContext context)
        {
            _context = context;
        }
        public async Task<Response> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return new Response(user.Id, user.Email, user.Name);
        }
    }

}
