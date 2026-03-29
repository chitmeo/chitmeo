using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Application.Configurations;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class GoogleLink
{
    
    public sealed class Command : IRequest<bool>
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }

    internal class Handler : IRequestHandler<Command, bool>
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

        public async Task<bool> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);                       

            return true;
        }
    }

}
