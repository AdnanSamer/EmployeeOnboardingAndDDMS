using EmployeeOnboarding_DDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeOnboarding_DDMS.Infrastructure.Configurations
{
    public class OnboardingTaskConfiguration : IEntityTypeConfiguration<OnboardingTask>
    {
        public void Configure(EntityTypeBuilder<OnboardingTask> builder)
        {
            builder.ToTable("OnboardingTasks");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.EmployeeId)
                .IsRequired();

            builder.Property(t => t.TaskTemplateId)
                .IsRequired();

            builder.Property(t => t.AssignedBy)
                .IsRequired();

            builder.Property(t => t.AssignedDate)
                .IsRequired();

            builder.Property(t => t.DueDate)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.Notes)
                .HasMaxLength(2000);

            // Relationships
            builder.HasOne(t => t.Employee)
                .WithMany(e => e.OnboardingTasks)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.TaskTemplate)
                .WithMany(tt => tt.OnboardingTasks)
                .HasForeignKey(t => t.TaskTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.AssignedByUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Documents)
                .WithOne(d => d.OnboardingTask)
                .HasForeignKey(d => d.OnboardingTaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

