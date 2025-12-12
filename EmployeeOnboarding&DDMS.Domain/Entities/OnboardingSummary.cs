using EmployeeOnboarding_DDMS.Domain.Common;

namespace EmployeeOnboarding_DDMS.Domain.Entities
{
    public class OnboardingSummary : AuditableBaseEntity
    {
        public int EmployeeId { get; set; }
        public int GeneratedBy { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string PdfFilePath { get; set; } = string.Empty;
        public string? SummaryData { get; set; } // JSON serialized data

        // Navigation properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual User GeneratedByUser { get; set; } = null!;
    }
}

