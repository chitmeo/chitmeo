using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Shared.Helpers;

namespace ChitMeo.Module.Auth.Application.UseCases.Users.Queries;

public static class GetUserById
{
    public record Response(Guid Id, string Email, string Name);
    public sealed class Query : IRequest<Response>
    {
        [Required(ErrorMessage = "UserId is required.")]
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
            ValidationHelper.ValidateAndThrow(request);
            var user = await ValidateAndThrowAsync(request, cancellationToken);
            return new Response(user.Id, user.Email, user.Name);
        }

        private async Task<Domain.Entities.User> ValidateAndThrowAsync(Query request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                throw new ValidationException("UserId is required.");
            }

            var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            return user;
        }
    }

}
