using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;

namespace ChitMeo.Module.Auth.Application.UseCases.Systems.Queries;

public static class GetScript
{
    public sealed record Result(string Script);
    public sealed record Request() : IRequest<Result>;

    internal class Handler : IRequestHandler<Request, Result>
    {
        private readonly IAuthDbContext _context;

        public Handler(IAuthDbContext context)
        {
            _context = context;
        }

        public async Task<Result> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            // Implement your logic to get the API script here
            var scriptContent = _context.GenerateCreateScript();
            return new Result(Script: scriptContent);
        }
    }
}
