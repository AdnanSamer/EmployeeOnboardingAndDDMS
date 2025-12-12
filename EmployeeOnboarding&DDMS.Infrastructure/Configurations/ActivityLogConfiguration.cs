using EmployeeOnboarding_DDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeOnboarding_DDMS.Infrastructure.Configurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.Property(a => a.UserName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityName)
                .HasMaxLength(200);

            builder.Property(a => a.IpAddress)
                .HasMaxLength(64);

            builder.Property(a => a.Timestamp)
                .IsRequired();
        }
    }
}

