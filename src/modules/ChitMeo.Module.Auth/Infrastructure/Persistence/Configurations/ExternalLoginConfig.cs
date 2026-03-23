using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChitMeo.Module.Auth.Infrastructure.Persistence.Configurations;

internal class ExternalLoginConfig : IEntityTypeConfiguration<ExternalLogin>
{
    public void Configure(EntityTypeBuilder<ExternalLogin> builder)
    {
        builder.ToTable($"{Constraints.PrefixTable}{nameof(ExternalLogin)}");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Provider).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ProviderUserId).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(255);
    }
}
