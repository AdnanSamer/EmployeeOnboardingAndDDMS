using EmployeeOnboarding_DDMS.Aplication.DTOs.Documents;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Http;

namespace EmployeeOnboarding_DDMS.Aplication.Interfaces
{
    public interface IDocumentService
    {
        Task<Response<DocumentDto>> UploadDocumentAsync(int taskId, IFormFile file, int uploadedBy);
        Task<Response<DocumentDto>> GetDocumentByIdAsync(int id);
        Task<Response<IEnumerable<DocumentDto>>> GetTaskDocumentsAsync(int taskId);
        Task<Response<byte[]>> GetDocumentFileAsync(int id);
        Task<Response<DocumentDto>> ReviewDocumentAsync(int id, ReviewDocumentDto dto);
        Task<Response<bool>> DeleteDocumentAsync(int id);
    }
}

