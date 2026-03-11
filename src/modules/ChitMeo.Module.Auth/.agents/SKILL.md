---
name: chitmeo-module-auth
description: 'ChitMeo.Module.Auth development. Use for: implementing authentication endpoints, adding JWT token management, user registration and login, email verification, external provider integration (Google), refresh token handling, password management, user sessions, and securing endpoints with authentication/authorization policies.'
argument-hint: 'Specify the authentication feature you want to add or modify'
---

# ChitMeo.Module.Auth Development

The **Auth module** is responsible for all authentication and authorization concerns in ChitMeo, including user identity management, JWT token generation, external provider integration, and session management.

## Module Overview

### Purpose
The Auth module provides a complete authentication system with:
- **User Management**: Registration, profile updates, account activation
- **JWT Authentication**: Token generation, validation, and refresh
- **Password Security**: Password hashing, reset flows
- **External Providers**: OAuth integration (Google)
- **Email Verification**: Confirmation flows for new accounts
- **User Sessions**: Track active sessions and force logout

### Architecture
```
ChitMeo.Module.Auth/
├── AuthModule.cs              # IModule implementation
├── DependencyInjection.cs     # Service registration
├── Api/                        # HTTP endpoints (vertical slices)
│   ├── Login/                 # Login endpoints
│   ├── Logout/                # Logout endpoints
│   ├── User/                  # User profile endpoints
│   ├── Token/                 # Token refresh/validation
│   ├── Link/                  # Link external providers
│   ├── Unlink/                # Unlink external providers
│   └── System/                # System endpoints
├── Application/               # Business logic (CQRS)
│   ├── Abstractions/          # Service interfaces (ITokenService, etc.)
│   ├── Configurations/        # JwtOptions, GoogleOptions
│   └── UseCases/              # Commands and handlers
│       ├── Auths/             # Login, logout, token operations
│       ├── Users/             # User management operations
│       ├── Tokens/            # Token operations
│       └── Systems/           # System-level operations
├── Domain/                    # Domain entities & business rules
│   └── Entities/
│       ├── User.cs            # Core user entity
│       ├── UserPassword.cs    # Password hashing/verification
│       ├── ExternalLogin.cs   # OAuth provider links
│       ├── RefreshToken.cs    # Token refresh management
│       ├── EmailVerification.cs # Email confirmation tokens
│       └── UserSession.cs     # Active session tracking
└── Infrastructure/            # Data access & external services
    ├── Persistence/           # EF Core DbContext
    └── Security/              # JWT, OAuth implementations
```

### Key Components

#### Domain Entities
- **User**: Core user account with email, name, activation status
- **UserPassword**: Hashed password storage and verification
- **ExternalLogin**: OAuth provider associations (Google, etc.)
- **RefreshToken**: Long-lived tokens for obtaining new access tokens
- **EmailVerification**: Tokens for email confirmation flows
- **UserSession**: Active session tracking and device management

#### Services
- **ITokenService**: JWT token generation and validation
- **IAuthDbContext**: Database context for auth data persistence
- **IUserService**: User account management operations
- **IExternalLoginService**: OAuth provider handling

#### Authentication Flow
```
1. User provides credentials (email + password)
   ↓
2. LoginWithPasswordEndpoint receives request
   ↓
3. PasswordLogin.Command dispatched via mediator
   ↓
4. PasswordLoginCommandHandler validates credentials
   ↓
5. JwtTokenService generates access + refresh tokens
   ↓
6. Response includes both tokens for client
   ↓
7. Client stores tokens (access in memory, refresh in secure storage)
```

## Route Prefix
All Auth endpoints are under `/auth` by default.

**Existing Endpoints:**
- `POST /auth/login/password` - Login with email/password
- `POST /auth/login/google` - Login with Google OAuth
- `POST /auth/logout` - Logout (invalidate tokens)
- `POST /auth/token/refresh` - Get new access token
- `POST /auth/user` - Create new user
- `GET /auth/user/{id}` - Get user profile
- `PUT /auth/user/{id}` - Update user profile
- `POST /auth/link/{provider}` - Link external provider
- `DELETE /auth/unlink/{provider}` - Unlink external provider

## When to Use This Skill

- **Add a new login endpoint** for a different provider (GitHub, Facebook, etc.)
- **Implement password reset** flow with email verification
- **Add user registration** with validation and email confirmation
- **Customize JWT claims** to include additional user roles/permissions
- **Add two-factor authentication** (2FA/MFA)
- **Implement role-based access control** (RBAC) with claims
- **Add device management** for user sessions
- **Implement rate limiting** for login attempts
- **Add OAuth provider configuration** in appsettings
- **Create user deactivation** or soft-delete functionality
- **Extend UserSession** for device tracking/management

## Step-by-Step Procedures

### 1. Add a New Login Endpoint

**Step 1: Create the command and handler**

File: `Application/UseCases/Auths/Commands/ProviderLogin.cs`

```csharp
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Domain.Entities;

namespace ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;

public static class ProviderLogin
{
    public record Command(string ProviderToken) : ICommand<Response>;
    
    public record Response(
        Guid UserId,
        string AccessToken,
        string? RefreshToken = null,
        bool IsNewUser = false
    );
    
    public class Handler : ICommandHandler<Command, Response>
    {
        private readonly IAuthDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IExternalProviderService _providerService;
        
        public Handler(IAuthDbContext context, ITokenService tokenService, IExternalProviderService providerService)
        {
            _context = context;
            _tokenService = tokenService;
            _providerService = providerService;
        }
        
        public async Task<Response> HandleAsync(Command command)
        {
            // Verify provider token
            var providerInfo = await _providerService.VerifyTokenAsync(command.ProviderToken);
            
            // Look for existing external login
            var externalLogin = await _context.ExternalLogins
                .FirstOrDefaultAsync(x => x.ProviderId == providerInfo.Id && x.Provider == "GitHub");
            
            bool isNewUser = externalLogin == null;
            User user;
            
            if (isNewUser)
            {
                // Create new user
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = providerInfo.Email,
                    Name = providerInfo.Name,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                _context.Users.Add(user);
                
                // Link external provider
                var link = new ExternalLogin
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Provider = "GitHub",
                    ProviderId = providerInfo.Id,
                    Email = providerInfo.Email,
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.ExternalLogins.Add(link);
            }
            else
            {
                user = await _context.Users.FindAsync(externalLogin.UserId);
            }
            
            await _context.SaveChangesAsync();
            
            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user);
            
            return new Response(user.Id, accessToken, refreshToken, isNewUser);
        }
    }
}
```

**Step 2: Create the endpoint**

File: `Api/Login/GitHubLoginEndpoint.cs`

```csharp
using ChitMeo.Mediator;
using ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;
using ChitMeo.Shared.Abstractions.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChitMeo.Module.Auth.Api.Login;

public class GitHubLoginEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login/github", HandleAsync)
            .WithName("LoginWithGitHub")
            .WithTags("Authentication")
            .WithSummary("Login with GitHub OAuth")
            .WithDescription("Authenticate using GitHub. Creates new user if OAuth email is not registered.")
            .AllowAnonymous();
    }
    
    private static async Task<IResult> HandleAsync(
        ProviderLogin.Command command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(command, cancellationToken);
        return Results.Ok(result);
    }
}
```

**Step 3: Update DependencyInjection.cs**

Add the provider service:

```csharp
public static void AddServices(this IServiceCollection services)
{
    services.AddScoped<ITokenService, JwtTokenService>();
    services.AddScoped<IExternalProviderService, ExternalProviderService>(); // Add this
}
```

### 2. Implement Password Reset Flow

**Step 1: Create reset token command**

File: `Application/UseCases/Users/Commands/RequestPasswordReset.cs`

```csharp
using ChitMeo.Mediator;

namespace ChitMeo.Module.Auth.Application.UseCases.Users.Commands;

public static class RequestPasswordReset
{
    public record Command(string Email) : ICommand<Response>;
    
    public record Response(bool Success, string Message);
    
    public class Handler : ICommandHandler<Command, Response>
    {
        private readonly IAuthDbContext _context;
        private readonly IEmailService _emailService;
        
        public async Task<Response> HandleAsync(Command command)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
            
            if (user == null)
            {
                // Don't reveal if user exists (security)
                return new Response(true, "If email exists, reset instructions have been sent");
            }
            
            // Create reset token
            var resetToken = Guid.NewGuid().ToString();
            var verification = new EmailVerification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Email = user.Email,
                Token = resetToken,
                Type = "PasswordReset",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            };
            
            _context.EmailVerifications.Add(verification);
            await _context.SaveChangesAsync();
            
            // Send email with reset link
            var resetLink = $"https://yourapp.com/reset-password?token={resetToken}";
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
            
            return new Response(true, "If email exists, reset instructions have been sent");
        }
    }
}
```

**Step 2: Create reset password confirmation command**

File: `Application/UseCases/Users/Commands/ResetPassword.cs`

```csharp
public static class ResetPassword
{
    public record Command(string Token, string NewPassword) : ICommand<Response>;
    
    public record Response(bool Success, string Message);
    
    public class Handler : ICommandHandler<Command, Response>
    {
        private readonly IAuthDbContext _context;
        
        public async Task<Response> HandleAsync(Command command)
        {
            var verification = await _context.EmailVerifications
                .FirstOrDefaultAsync(v => v.Token == command.Token && v.Type == "PasswordReset");
            
            if (verification == null || verification.ExpiresAt < DateTime.UtcNow)
                return new Response(false, "Reset token is invalid or expired");
            
            var user = await _context.Users.FindAsync(verification.UserId);
            
            // Hash new password
            var userPassword = new UserPassword
            {
                UserId = user.Id,
                Hash = PasswordHasher.Hash(command.NewPassword),
                CreatedAt = DateTime.UtcNow
            };
            
            _context.UserPasswords.Add(userPassword);
            _context.EmailVerifications.Remove(verification);
            await _context.SaveChangesAsync();
            
            return new Response(true, "Password has been reset successfully");
        }
    }
}
```

### 3. Add Enhanced JWT Claims

**File: `Infrastructure/Security/JwtTokenService.cs`**

Extend claims to include roles, permissions, etc.:

```csharp
public string GenerateAccessToken(User user, IEnumerable<string>? roles = null)
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("EmailConfirmed", user.EmailConfirmed.ToString()),
        new Claim("IsActive", user.IsActive.ToString()),
    };
    
    // Add roles as separate claims
    if (roles != null)
    {
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
    }
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
    var token = new JwtSecurityToken(
        issuer: _jwtOptions.Issuer,
        audience: _jwtOptions.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddDays(_jwtOptions.ExpireDays),
        signingCredentials: creds
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

### 4. Add Email Verification for New Users

**File: `Application/UseCases/Users/Commands/RegisterUser.cs`**

```csharp
public static class RegisterUser
{
    public record Command(string Email, string Name, string Password) : ICommand<Response>;
    
    public record Response(Guid UserId, string Message);
    
    public class Handler : ICommandHandler<Command, Response>
    {
        private readonly IAuthDbContext _context;
        private readonly IEmailService _emailService;
        
        public async Task<Response> HandleAsync(Command command)
        {
            // Check if user exists
            if (await _context.Users.AnyAsync(u => u.Email == command.Email))
                throw new InvalidOperationException("Email already registered");
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = command.Email,
                Name = command.Name,
                EmailConfirmed = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            // Hash password
            var userPassword = new UserPassword
            {
                UserId = user.Id,
                Hash = PasswordHasher.Hash(command.Password),
                CreatedAt = DateTime.UtcNow
            };
            
            // Create verification token
            var verificationToken = Guid.NewGuid().ToString();
            var emailVerification = new EmailVerification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Email = command.Email,
                Token = verificationToken,
                Type = "EmailConfirmation",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(user);
            _context.UserPasswords.Add(userPassword);
            _context.EmailVerifications.Add(emailVerification);
            await _context.SaveChangesAsync();
            
            // Send verification email
            var confirmLink = $"https://yourapp.com/confirm-email?token={verificationToken}";
            await _emailService.SendConfirmationEmailAsync(user.Email, confirmLink);
            
            return new Response(user.Id, "User registered. Please verify your email.");
        }
    }
}
```

### 5. Customize Authentication Configuration

**In `appsettings.json`:**

```json
{
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long",
    "Issuer": "ChitMeo",
    "Audience": "ChitMeoUsers",
    "ExpireDays": 7,
    "RefreshTokenExpireDays": 30
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    },
    "GitHub": {
      "ClientId": "your-github-client-id",
      "ClientSecret": "your-github-client-secret"
    }
  },
  "Email": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "FromAddress": "noreply@yourapp.com",
    "Username": "your-email@example.com",
    "Password": "your-password"
  }
}
```

## Domain Model Reference

### User Entity
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }           // Unique email
    public string Name { get; set; }            // Display name
    public bool EmailConfirmed { get; set; }    // Email verified
    public bool IsActive { get; set; }          // Account active
    public DateTime CreatedAt { get; set; }     // Registration date
    public DateTime UpdatedAt { get; set; }     // Last updated
}
```

### UserPassword Entity
```csharp
public class UserPassword
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Hash { get; set; }            // Bcrypt/Argon2 hash
    public DateTime CreatedAt { get; set; }
    public DateTime? InvalidatedAt { get; set; } // Null if current
}
```

### ExternalLogin Entity
```csharp
public class ExternalLogin
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; }        // "Google", "GitHub", etc.
    public string ProviderId { get; set; }      // Provider's user ID
    public string Email { get; set; }           // Provider's email
    public DateTime CreatedAt { get; set; }
}
```

### RefreshToken Entity
```csharp
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### EmailVerification Entity
```csharp
public class EmailVerification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string Type { get; set; }            // "EmailConfirmation", "PasswordReset"
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### UserSession Entity
```csharp
public class UserSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DeviceName { get; set; }      // Device identifier
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime LastActivityAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## Key Services

### ITokenService
```csharp
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    ClaimsPrincipal ValidateToken(string token);
}
```

### IAuthDbContext
```csharp
public interface IAuthDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserPassword> UserPasswords { get; }
    DbSet<ExternalLogin> ExternalLogins { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<EmailVerification> EmailVerifications { get; }
    DbSet<UserSession> UserSessions { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

## Configuration Reference

### JwtOptions
```csharp
public class JwtOptions
{
    public string Key { get; set; }              // Secret signing key
    public string Issuer { get; set; }           // Token issuer
    public string Audience { get; set; }         // Token audience
    public int ExpireDays { get; set; }          // Access token lifetime
    public int RefreshTokenExpireDays { get; set; } // Refresh token lifetime
}
```

### GoogleOptions
```csharp
public class GoogleOptions
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
```

## Common Patterns

### Custom Claims in JWT
Add custom claims at token generation to control per-user features:

```csharp
claims.Add(new Claim("Subscription", user.SubscriptionLevel));
claims.Add(new Claim("Features", string.Join(",", user.EnabledFeatures)));
```

### Refresh Token Rotation
Implement security best practice of rotating refresh tokens:

```csharp
public async Task<(string AccessToken, string NewRefreshToken)> RefreshAsync(string refreshToken)
{
    var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
    
    if (token?.ExpiresAt < DateTime.UtcNow || token?.IsRevoked == true)
        throw new UnauthorizedAccessException("Invalid refresh token");
    
    var user = await _context.Users.FindAsync(token.UserId);
    
    // Revoke old token
    token.IsRevoked = true;
    
    // Generate new tokens
    var newAccessToken = _tokenService.GenerateAccessToken(user);
    var newRefreshToken = _tokenService.GenerateRefreshToken(user);
    
    await _context.SaveChangesAsync();
    
    return (newAccessToken, newRefreshToken);
}
```

### Password Hashing
Always use strong hashing algorithms (never plain MD5/SHA1):

```csharp
// Use BCrypt or Argon2
using BCrypt.Net;

var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12);
bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
```

### Rate Limiting on Login
Prevent brute force attacks:

```csharp
public async Task<Response> HandleAsync(Command command)
{
    var recentAttempts = await _context.LoginAttempts
        .Where(a => a.Email == command.Email && a.Timestamp > DateTime.UtcNow.AddMinutes(-15))
        .CountAsync();
    
    if (recentAttempts >= 5)
        throw new InvalidOperationException("Too many login attempts. Try again in 15 minutes.");
    
    // ... rest of login logic
}
```

## Testing Endpoints

### Test Login Endpoint
```bash
curl -X POST http://localhost:5000/auth/login/password \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'
```

### Test Token Refresh
```bash
curl -X POST http://localhost:5000/auth/token/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"your-refresh-token"}'
```

### Use Access Token
```bash
curl -X GET http://localhost:5000/api/protected-resource \
  -H "Authorization: Bearer {accessToken}"
```

## Best Practices

1. **Never log passwords** - Even in debug logs
2. **Use HTTPS only** - Protect tokens in transit
3. **Store tokens securely** - Use httpOnly cookies for refresh tokens
4. **Implement token rotation** - Revoke old refresh tokens
5. **Validate on every request** - Check token expiration and signature
6. **Use strong JWT keys** - Minimum 256 bits (32 characters)
7. **Implement rate limiting** - Prevent credential stuffing attacks
8. **Log authentication events** - Track login/logout for audit
9. **Expire tokens properly** - Short-lived access tokens, long-lived refresh tokens
10. **Validate email addresses** - Confirm ownership before critical operations

## References

- [AuthModule.cs](../AuthModule.cs) - Module configuration and endpoint mapping
- [JwtTokenService.cs](../Infrastructure/Security/JwtTokenService.cs) - Token generation logic
- [PasswordLoginCommandHandler](../Application/UseCases/Auths/Commands) - Login implementation
- [AuthDbContext.cs](../Infrastructure/Persistence/AuthDbContext.cs) - Database schema
- [DependencyInjection.cs](../DependencyInjection.cs) - Service registration
