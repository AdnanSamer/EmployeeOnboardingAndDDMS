namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Admin
{
    public class OverdueTaskAlertDto
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
        public int DaysOverdue { get; set; }
    }

    public class RejectedDocumentAlertDto
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime UploadedDate { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
    }
}

