using Microsoft.AspNetCore.Http;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Documents
{
    public class UploadDocumentRequestDto
    {
        public IFormFile File { get; set; } = null!;
        public int UploadedBy { get; set; }
    }
}

