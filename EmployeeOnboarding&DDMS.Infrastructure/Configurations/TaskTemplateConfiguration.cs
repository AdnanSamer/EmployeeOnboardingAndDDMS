using EmployeeOnboarding_DDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeOnboarding_DDMS.Infrastructure.Configurations
{
    public class TaskTemplateConfiguration : IEntityTypeConfiguration<TaskTemplate>
    {
        public void Configure(EntityTypeBuilder<TaskTemplate> builder)
        {
            builder.ToTable("TaskTemplates");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(4000);

            builder.Property(t => t.IsRequired)
                .HasDefaultValue(true);

            builder.Property(t => t.RequiresDocumentUpload)
                .HasDefaultValue(false);

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true);

            // Relationships
            builder.HasMany(t => t.OnboardingTasks)
                .WithOne(ot => ot.TaskTemplate)
                .HasForeignKey(ot => ot.TaskTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

