using ChitMeo.Shared.Abstractions.Modules;
using ChitMeo.Shared.Infrastructure.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Module.Accounting;

public class AccountingModule : IModule
{
    public string Name => "Accounting";

    public string RoutePrefix => "/accg";

    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup(RoutePrefix)
            .WithTags(Name);

        group.MapEndpointsFromAssembly(typeof(AccountingModule).Assembly);
    }
}
