using ChitMeo.Module.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChitMeo.Module.Auth.Infrastructure.Persistence.Configurations;

internal class EmailVerificationConfig: IEntityTypeConfiguration<EmailVerification>
{
    public void Configure(EntityTypeBuilder<EmailVerification> builder)
    {
        builder.ToTable($"{Constraints.PrefixTable}{nameof(EmailVerification)}");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }
}
