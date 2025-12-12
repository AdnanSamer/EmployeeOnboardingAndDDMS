using EmployeeOnboarding_DDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeOnboarding_DDMS.Infrastructure.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.EmployeeNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(e => e.EmployeeNumber)
                .IsUnique();

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(e => e.Email)
                .IsUnique();

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(e => e.HireDate)
                .IsRequired();

            builder.Property(e => e.Department)
                .HasMaxLength(100);

            builder.Property(e => e.Position)
                .HasMaxLength(100);

            builder.Property(e => e.EmploymentStatus)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.OnboardingStatus)
                .IsRequired()
                .HasConversion<int>();

            // Relationships
            builder.HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(e => e.OnboardingTasks)
                .WithOne(t => t.Employee)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.OnboardingSummaries)
                .WithOne(s => s.Employee)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

