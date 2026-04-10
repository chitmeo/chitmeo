---
name: chitmeo-development
description: 'ChitMeo modular ASP.NET Core development. Use for: creating modules, adding endpoints, configuring dependency injection, implementing CQRS handlers, adding database contexts, managing module dependencies, setting up authentication, and following vertical slice architecture patterns.'
argument-hint: 'Specify the module or feature you want to add'
---

# ChitMeo Development

ChitMeo is a **lightweight modular platform** for building applications with **independent modules** on ASP.NET Core. This skill provides patterns, conventions, and procedures for developing ChitMeo modules.

## Architecture Overview

### Key Principles
- **Modular Architecture**: Independent, self-contained modules
- **Loose Coupling**: Modules communicate via shared abstractions, not direct references
- **Vertical Slice**: Each module owns its own domain logic, API endpoints, handlers, and infrastructure
- **CQRS**: Custom lightweight mediator pattern for commands/queries
- **Convention-based Loading**: Host automatically discovers and loads `ChitMeo.Module.*` assemblies

### Project Structure
```
src/
├── ChitMeo.Host/              # ASP.NET Core runtime container
│   ├── Program.cs            # Bootstrap and module loading
│   ├── Extensions/           # Builder/App configuration extensions
│   └── appsettings.json      # Module enablement and platform config
│
├── ChitMeo.Shared/           # Shared contracts (no business logic)
│   └── Abstractions/         # IModule, IEndpoint, DependsOn attribute, etc.
│
└── modules/                   # Independent modules
    ├── ChitMeo.Module.Auth/
    ├── ChitMeo.Module.Blog/
    └── ChitMeo.Module.Accounting/
```

## When to Use This Skill

- **Creating a new module** from scratch
- **Adding endpoints** to an existing module
- **Implementing handlers** for CQRS commands/queries
- **Configuring dependency injection** for a module
- **Adding database contexts** with EF Core
- **Setting up module dependencies** with `DependsOn` attribute
- **Integrating authentication/authorization** into a module
- **Registering services** and extending middlewares

## Core Concepts

### IModule Interface
Every module must implement `IModule` from `ChitMeo.Shared.Abstractions.Modules`:

```csharp
public interface IModule
{
    string Name { get; }                                          // Unique module name
    string RoutePrefix { get; }                                   // Base API route prefix
    void ConfigureServices(IServiceCollection services, IConfiguration config);
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
```

**Example** (AuthModule):
```csharp
public class AuthModule : IModule
{
    public string Name => "Auth";
    public string RoutePrefix => "/auth";

    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection("Jwt"));
        services.AddDbContext(config);
        services.AddServices();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(RoutePrefix).WithName(Name);
        // Maps endpoints here
    }
}
```

### IEndpoint Interface
Endpoints are self-contained units that map routes:

```csharp
public interface IEndpoint
{
    void Map(RouteGroupBuilder group);
}
```

**Example** (Login endpoint):
```csharp
public class LoginEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", HandleAsync)
            .WithName("Login")
            .WithOpenApi();
    }

    private static async Task<IResult> HandleAsync(LoginRequest req, IMediator mediator)
    {
        var result = await mediator.SendAsync(new LoginCommand(req.Email, req.Password));
        return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Error);
    }
}
```

### CQRS Pattern
Use the lightweight custom mediator for commands and queries:

```csharp
// Command
public record CreateUserCommand(string Email, string Password) : ICommand<User>;

// Handler
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, User>
{
    public async Task<User> HandleAsync(CreateUserCommand command)
    {
        // Implementation
    }
}

// Usage
var user = await mediator.SendAsync(new CreateUserCommand("user@example.com", "password"));
```

### Handler Validation Pattern
In command handlers, separate request validation from business rule validation and keep `HandleAsync` focused on the main business flow.

```csharp
public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthResponse>
{
    public async Task<AuthResponse> HandleAsync(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var payload = await ValidateAsync(request, cancellationToken);  // validate external token and business invariants

        // Main business logic
        var user = CreateUserFromPayload(payload);
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponse(accessToken);
    }

    private async Task<Payload> ValidateAsync(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        // Validate Google token and get provider payload
        var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);

        // Check business rules, e.g. existing user/email collisions
        var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == payload.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email {payload.Email} already exists.");
        }

        return payload;
    }
}
```

- `ValidationHelper.ValidateAndThrow(request)` checks request model validation attributes.
- `ValidateAsync(...)` is a separate method for business/domain validation, including external token validation.
- `HandleAsync` should orchestrate the main business flow after all validation is complete.

### Module Dependencies
Use `DependsOn` attribute to declare module dependencies:

```csharp
[DependsOn(typeof(AuthModule))]
public class BlogModule : IModule
{
    // BlogModule depends on Auth being loaded first
}
```

## Step-by-Step Procedures

### 1. Create a New Module

1. Create folder: `src/modules/ChitMeo.Module.YourModule/`
2. Create `.csproj` file with references to `ChitMeo.Shared`
3. Create module class implementing `IModule`:

```csharp
using ChitMeo.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Module.YourModule;

public class YourModule : IModule
{
    public string Name => "YourModule";
    public string RoutePrefix => "/yourmodule";

    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        // Register services, DbContexts, etc.
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(RoutePrefix).WithName(Name);
        // Map endpoints here
    }
}
```

4. Enable in `appsettings.json`:
```json
{
  "Modules": {
    "YourModule": true
  }
}
```

### 2. Add an Endpoint

1. Create file: `Api/YourFeature/YourEndpoint.cs`
2. Implement `IEndpoint`:

```csharp
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.YourModule.Api.YourFeature;

public class CreateItemEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/items", HandleAsync)
            .WithName("CreateItem")
            .WithOpenApi();
    }

    private static async Task<IResult> HandleAsync(CreateItemRequest request, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.SendAsync(new CreateItemCommand(request.Name), ct);
        return result.IsSuccess ? Results.Created($"/items/{result.Data.Id}", result.Data) : Results.BadRequest();
    }
}
```

3. Register in module's `MapEndpoints`:

```csharp
public void MapEndpoints(IEndpointRouteBuilder endpoints)
{
    var group = endpoints.MapGroup(RoutePrefix).WithName(Name);
    group.MapEndpoint<CreateItemEndpoint>();
    group.MapEndpoint<GetItemEndpoint>();
    // ... other endpoints
}
```

### 3. Implement CQRS Handler

1. Create command/query: `Application/UseCases/CreateItem/CreateItemCommand.cs`

```csharp
using ChitMeo.Mediator;

namespace ChitMeo.Module.YourModule.Application.UseCases.CreateItem;

public record CreateItemCommand(string Name) : ICommand<ItemResult>;
```

2. Create handler: `Application/UseCases/CreateItem/CreateItemCommandHandler.cs`

```csharp
using ChitMeo.Mediator;

namespace ChitMeo.Module.YourModule.Application.UseCases.CreateItem;

public class CreateItemCommandHandler : ICommandHandler<CreateItemCommand, ItemResult>
{
    private readonly IYourModuleDbContext _context;

    public CreateItemCommandHandler(IYourModuleDbContext context)
    {
        _context = context;
    }

    public async Task<ItemResult> HandleAsync(CreateItemCommand command)
    {
        var item = new Item { Name = command.Name };
        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return new ItemResult(item.Id, item.Name);
    }
}
```

### 4. Configure Dependency Injection

Create `DependencyInjection.cs` in module root:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ChitMeo.Module.YourModule;

public static class DependencyInjection
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IYourService, YourService>();
    }

    public static void AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("YourModule");
        services.AddDbContext<YourModuleDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}
```

Call from `YourModule.ConfigureServices`:

```csharp
public void ConfigureServices(IServiceCollection services, IConfiguration config)
{
    services.AddDbContext(config);
    services.AddServices();
}
```

### 5. Set Up Database Context

1. Create `Infrastructure/Persistence/YourModuleDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.YourModule.Infrastructure.Persistence;

public interface IYourModuleDbContext
{
    DbSet<Item> Items { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class YourModuleDbContext : DbContext, IYourModuleDbContext
{
    public DbSet<Item> Items { get; set; }

    public YourModuleDbContext(DbContextOptions<YourModuleDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ItemConfiguration());
    }
}
```

2. Configure in `DependencyInjection.cs`:

```csharp
public static void AddDbContext(this IServiceCollection services, IConfiguration config)
{
    var dataConnectionOptions = config.GetSection("DataConnection").Get<DataConnectionOptions>();

    switch (dataConnectionOptions.Provider)
    {
        case DataProvider.MySql:
            services.AddScoped<IYourModuleDbContext, YourModuleDbContext>(sp =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<YourModuleDbContext>();
                optionsBuilder.UseMySQL(dataConnectionOptions.ConnectionString);
                return new YourModuleDbContext(optionsBuilder.Options);
            });
            break;
        // ... other providers
    }
}
```

## Key Patterns & Conventions

### Naming Conventions
- **Modules**: `ChitMeo.Module.{FeatureName}`
- **Commands/Queries**: `{Action}Command` or `{Action}Query` (e.g., `CreateUserCommand`, `GetUserQuery`)
- **Handlers**: `{CommandOrQuery}Handler` (e.g., `CreateUserCommandHandler`)
- **Endpoints**: `{Action}Endpoint` (e.g., `CreateUserEndpoint`, `LoginEndpoint`)
- **DbContext**: `{Feature}DbContext` (e.g., `AuthDbContext`, `BlogDbContext`)
- **Routes**: Plural nouns for collections `/users`, `/blog-posts`

### Folder Structure (Module Template)
```
ChitMeo.Module.YourModule/
├── YourModule.cs                 # IModule implementation
├── DependencyInjection.cs        # Service registration
├── Api/
│   └── YourFeature/
│       ├── YourEndpoint.cs       # IEndpoint implementation
│       └── Requests/
├── Application/
│   ├── Abstractions/             # Service interfaces
│   └── UseCases/
│       └── CreateItem/
│           ├── CreateItemCommand.cs
│           ├── CreateItemCommandHandler.cs
│           └── CreateItemRequest.cs
├── Domain/
│   └── Entities/                 # Domain models
├── Infrastructure/
│   ├── Persistence/              # EF Core DbContext
│   └── Security/                 # Auth/security implementations
└── ChitMeo.Module.YourModule.csproj
```

### Configuration Management
Modules typically read from `appsettings.json`:

```csharp
public void ConfigureServices(IServiceCollection services, IConfiguration config)
{
    services.Configure<JwtOptions>(config.GetSection("Jwt"));
    services.Configure<EmailOptions>(config.GetSection("Email"));
    // Use IOptions<T> in handlers/services
}
```

### Error Handling
Use result pattern for operations:

```csharp
public record Result<T>(bool IsSuccess, T? Data, string? Error)
{
    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

### Testing
- Unit tests go in `test/` at workspace root
- Test module structure mirrors source structure
- Reference module via assembly loading (testing in isolation)

## Common Task Reference

| Task | File | Key Pattern |
|------|------|--------------|
| Add endpoint | `Api/{Feature}/{Action}Endpoint.cs` | Implement `IEndpoint`, map in `MapEndpoints()` |
| Add command handler | `Application/UseCases/{Feature}/{Command}Handler.cs` | Implement `ICommandHandler<TCommand, TResult>` |
| Add service | `Infrastructure/{Layer}/{Service}Service.cs` | Define interface in `Abstractions/`, register in `DependencyInjection.cs` |
| Add DbContext | `Infrastructure/Persistence/{Module}DbContext.cs` | Implement custom interface + DbContext, configure via `DependencyInjection.cs` |
| Configure DB provider | `DependencyInjection.cs` | Switch on `DataProvider` enum (MySQL, SqlServer, MariaDB) |
| Handle authentication | `Infrastructure/Security/{Feature}.cs` | Configure JWT/OAuth in `ConfigureServices()` |

## Best Practices

1. **Keep modules independent**: Minimize cross-module dependencies
2. **Use interfaces for abstraction**: Define contracts in Shared/Abstractions
3. **Vertical slices**: Group related code (endpoint, handler, domain) together
4. **Configuration-driven**: Use IOptions<T> and appsettings.json, not hardcoded values
5. **Database per module**: Modules can have separate databases (per configuration)
6. **DependsOn**: Declare module dependencies explicitly to control load order
7. **Validation ordering**: Validate request data first, then business/domain invariants in a separate helper method
8. **Error handling**: Always return results, don't throw exceptions for business logic
9. **Mediator pattern**: Use for all business logic to keep it isolated from endpoints

## Configuration Template

Add to your `appsettings.json` when creating new modules:

```json
{
  "Modules": {
    "YourModule": true
  },
  "YourModuleSettings": {
    "SomeOption": "value"
  }
}
```

## References

- [README.md](../../README.md) - Project overview and architecture
- [IModule.cs](../../src/ChitMeo.Shared/Abstractions/Modules/IModule.cs) - Module interface contract
- [IEndpoint.cs](../../src/ChitMeo.Shared/Abstractions/Endpoints/IEndpoint.cs) - Endpoint interface contract
- [Program.cs](../../src/ChitMeo.Host/Program.cs) - Host bootstrap and module loading logic
- [WebApplicationBuilderExtensions.cs](../../src/ChitMeo.Host/Extensions/WebApplicationBuilderExtensions.cs) - How modules are discovered and loaded
