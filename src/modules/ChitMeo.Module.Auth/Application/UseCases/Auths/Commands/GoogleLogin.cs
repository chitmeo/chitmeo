using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Application.Configurations;
using ChitMeo.Module.Auth.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class GoogleLogin
{
    public record AuthResponse(string AccessToken);
    public sealed class Command : IRequest<AuthResponse>
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }

    internal class Handler : IRequestHandler<Command, AuthResponse>
    {
        private readonly IAuthDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly GoogleOptions _google;

        public Handler(
            IAuthDbContext context,
            ITokenService tokenService,
            IOptions<GoogleOptions> googleOptions)
        {
            _context = context;
            _tokenService = tokenService;
            _google = googleOptions.Value;
        }

        public async Task<AuthResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            //ValidationHelper.ValidateAndThrow(request);

            //var payload = await GoogleJsonWebSignature.ValidateAsync(
            //    request.Token,
            //    new GoogleJsonWebSignature.ValidationSettings
            //    {
            //        Audience = new[] { _google.ClientId }
            //    });

            //var user = await _context.Users
            //    .FirstOrDefaultAsync(x => x.GoogleId == payload.Subject, cancellationToken);

            //if (user == null)
            //{
            //    user = new User
            //    {
            //        Id = Guid.NewGuid(),
            //        GoogleId = payload.Subject,
            //        Email = payload.Email,
            //        Name = payload.Name
            //    };

            //    await _context.Users.AddAsync(user, cancellationToken);
            //    await _context.SaveChangesAsync(cancellationToken);
            //}
            var user = new User();
            var accessToken = _tokenService.GenerateAccessToken(user);

            return new AuthResponse(accessToken);
        }

        public Task<AuthResponse> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}