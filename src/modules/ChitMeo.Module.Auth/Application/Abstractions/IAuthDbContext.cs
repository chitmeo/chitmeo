using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChitMeo.Module.Auth.Application.Abstractions;

public interface IAuthDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync();
    string GenerateCreateScript();
}
