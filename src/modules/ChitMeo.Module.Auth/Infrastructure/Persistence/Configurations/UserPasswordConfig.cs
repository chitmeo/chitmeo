
using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChitMeo.Module.Auth.Infrastructure.Persistence.Configurations;

internal class UserPasswordConfig : IEntityTypeConfiguration<UserPassword>
{
    public void Configure(EntityTypeBuilder<UserPassword> builder)
    {
        builder.ToTable($"{Constraints.PrefixTable}{nameof(UserPassword)}");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.PasswordSalt).IsRequired().HasMaxLength(32);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(1024);
    }
}
