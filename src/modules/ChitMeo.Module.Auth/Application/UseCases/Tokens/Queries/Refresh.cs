using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;

namespace ChitMeo.Module.Auth.Application.UseCases.Tokens.Queries;

public static class Refresh
{
    public record Response(string AccessToken, string RefreshToken);
    public sealed class Query : IRequest<Response>
    {
        public string RefreshToken { get; set; } = string.Empty;
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
            throw new NotImplementedException();
        }
    }

}
