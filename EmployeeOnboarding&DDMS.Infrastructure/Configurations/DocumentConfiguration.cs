using EmployeeOnboarding_DDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeOnboarding_DDMS.Infrastructure.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.OnboardingTaskId)
                .IsRequired();

            builder.Property(d => d.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(d => d.FileSize)
                .IsRequired();

            builder.Property(d => d.ContentType)
                .HasMaxLength(100)
                .HasDefaultValue("application/pdf");

            builder.Property(d => d.UploadedBy)
                .IsRequired();

            builder.Property(d => d.UploadDate)
                .IsRequired();

            builder.Property(d => d.Version)
                .HasDefaultValue(1);

            builder.Property(d => d.IsCurrentVersion)
                .HasDefaultValue(true);

            builder.Property(d => d.Comments)
                .HasMaxLength(2000);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(d => d.ReviewComments)
                .HasMaxLength(2000);

            builder.Property(d => d.AnnotationsJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(d => d.IsArchived)
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(d => d.OnboardingTask)
                .WithMany(t => t.Documents)
                .HasForeignKey(d => d.OnboardingTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.UploadedByUser)
                .WithMany()
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.ReviewedByUser)
                .WithMany()
                .HasForeignKey(d => d.ReviewedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

