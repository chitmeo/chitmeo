using System.Reflection;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.Auth.Infrastructure.Persistence;

public class AuthDbContext : DbContext, IAuthDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserPassword> UserPasswords => Set<UserPassword>();
    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailVerification> EmailVerifications => Set<EmailVerification>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public string GenerateCreateScript()
    {
        return base.Database.GenerateCreateScript();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}
