namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Documents
{
    public class DocumentVersionDto
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public List<VersionInfoDto> Versions { get; set; } = new();
    }

    public class VersionInfoDto
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public DateTime UploadDate { get; set; }
        public Domain.Enums.DocumentStatus Status { get; set; }
        public string? ReviewComments { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public long FileSize { get; set; }
        public bool IsCurrentVersion { get; set; }
    }
}
