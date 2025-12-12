using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Documents
{
    public class ReviewDocumentDto
    {
        public int DocumentId { get; set; }
        public DocumentStatus Status { get; set; }
        public string? Comments { get; set; }
        public int ReviewedBy { get; set; }
    }
}

