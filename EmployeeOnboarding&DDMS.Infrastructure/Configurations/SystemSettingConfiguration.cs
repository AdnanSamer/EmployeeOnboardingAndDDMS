using EmployeeOnboarding_DDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeOnboarding_DDMS.Infrastructure.Configurations
{
    public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
    {
        public void Configure(EntityTypeBuilder<SystemSetting> builder)
        {
            builder.Property(s => s.SettingKey)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.SettingValue)
                .IsRequired();

            builder.HasIndex(s => s.SettingKey)
                .IsUnique();
        }
    }
}

