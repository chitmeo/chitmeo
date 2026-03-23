using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChitMeo.Module.Auth.Infrastructure.Persistence.Configurations;

internal class UserSessionConfig : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable($"{Constraints.PrefixTable}{nameof(UserSession)}");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.UserAgent).IsRequired().HasMaxLength(512);
        builder.Property(x => x.Device).IsRequired().HasMaxLength(256);
        builder.Property(x => x.IPAddress).IsRequired().HasMaxLength(45);
    }
}
