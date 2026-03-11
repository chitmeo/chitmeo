using ChitMeo.Mediator;

namespace ChitMeo.Module.Auth.Application.UseCases.Systems.Queries;

public static class GetVersion
{
    public sealed record Result(string Module, string Version);
    public sealed record Command() : IRequest<Result>;

    internal class Handler : IRequestHandler<Command, Result>
    {

        public async Task<Result> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var result = new Result(
                Module: "ChitMeo.Module.Auth",
                Version: typeof(GetVersion).Assembly.GetName().Version?.ToString() ?? "Unknown"
            );
            return result;
        }
    }
}
