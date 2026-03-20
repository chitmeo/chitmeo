using System.Reflection;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.Auth.Infrastructure.Persistence;

public class AuthDbContext : DbContext,IAuthDbContext
{
    public DbSet<User> Users => Set<User>();

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
