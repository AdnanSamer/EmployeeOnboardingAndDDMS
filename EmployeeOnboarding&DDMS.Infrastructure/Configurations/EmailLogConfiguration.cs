using EmployeeOnboarding_DDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeOnboarding_DDMS.Infrastructure.Configurations
{
    public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
    {
        public void Configure(EntityTypeBuilder<EmailLog> builder)
        {
            builder.ToTable("EmailLogs");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.ToEmail)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Body)
                .IsRequired();

            builder.Property(e => e.EmailType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            builder.Property(e => e.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(e => e.Created)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}

