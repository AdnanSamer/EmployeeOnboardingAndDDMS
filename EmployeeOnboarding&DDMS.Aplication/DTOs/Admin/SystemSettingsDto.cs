namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Admin
{
    public class SystemSettingsDto
    {
        public TaskTemplateSettings TaskTemplateSettings { get; set; } = new();
        public DocumentSettings DocumentSettings { get; set; } = new();
        public NotificationSettings NotificationSettings { get; set; } = new();
        public ReportingSettings ReportingSettings { get; set; } = new();
    }

    public class TaskTemplateSettings
    {
        public bool AllowCustomTasks { get; set; } = true;
        public bool RequireDocumentUpload { get; set; } = false;
        public int DefaultDueDays { get; set; } = 7;
    }

    public class DocumentSettings
    {
        public int MaxFileSizeMB { get; set; } = 10;
        public List<string> AllowedFileTypes { get; set; } = new() { "PDF" };
        public string StorageType { get; set; } = "Local";
        public string? AzureBlobConnectionString { get; set; }
        public string? AzureBlobContainerName { get; set; }
        public int MetadataRetentionDays { get; set; } = 365;
    }

    public class NotificationSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public string SmtpFromEmail { get; set; } = string.Empty;
        public string SmtpFromName { get; set; } = "Employee Onboarding System";
        public bool EnableSsl { get; set; } = true;
        public bool EnableOverdueTaskEmails { get; set; } = true;
        public bool EnableDailySummaryEmails { get; set; } = false;
        public bool EnableCompletionEmails { get; set; } = true;
    }

    public class ReportingSettings
    {
        public int DocumentRetentionDays { get; set; } = 365;
        public List<string> ExportFormats { get; set; } = new() { "PDF", "Excel" };
        public bool AutoGenerateReports { get; set; } = false;
        public string? ReportSchedule { get; set; }
    }
}

