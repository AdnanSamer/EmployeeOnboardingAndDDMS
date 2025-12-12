using EmployeeOnboarding_DDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeOnboarding_DDMS.Infrastructure.Configurations
{
    public class OnboardingSummaryConfiguration : IEntityTypeConfiguration<OnboardingSummary>
    {
        public void Configure(EntityTypeBuilder<OnboardingSummary> builder)
        {
            builder.ToTable("OnboardingSummaries");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.EmployeeId)
                .IsRequired();

            builder.Property(s => s.GeneratedBy)
                .IsRequired();

            builder.Property(s => s.GeneratedDate)
                .IsRequired();

            builder.Property(s => s.PdfFilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(s => s.SummaryData)
                .HasMaxLength(4000);

            // Relationships
            builder.HasOne(s => s.Employee)
                .WithMany(e => e.OnboardingSummaries)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.GeneratedByUser)
                .WithMany()
                .HasForeignKey(s => s.GeneratedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

