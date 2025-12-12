using AutoMapper;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Documents;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Enums;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IOnboardingTaskRepository _taskRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        public DocumentService(
            IDocumentRepository documentRepository,
            IOnboardingTaskRepository taskRepository,
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IFileStorageService fileStorageService,
            IEmailService emailService,
            IMapper mapper)
        {
            _documentRepository = documentRepository;
            _taskRepository = taskRepository;
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<Response<DocumentDto>> UploadDocumentAsync(int taskId, IFormFile file, int uploadedBy)
        {
            // Validate file
            if (file == null || file.Length == 0)
            {
                return new Response<DocumentDto>("File is required.");
            }

            if (file.Length > MaxFileSize)
            {
                return new Response<DocumentDto>("File size exceeds 10 MB limit.");
            }

            // Validate file type - check both content type and extension
            var allowedContentTypes = new[] { "application/pdf" };
            var allowedExtensions = new[] { ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedContentTypes.Contains(file.ContentType.ToLower()) && 
                !allowedExtensions.Contains(fileExtension))
            {
                return new Response<DocumentDto>("Only PDF files are allowed.");
            }

            // Check if task exists
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                return new Response<DocumentDto>("Task not found.");
            }

            // Get current version
            var currentVersion = await _documentRepository.GetCurrentVersionAsync(taskId);
            var nextVersion = currentVersion != null ? currentVersion.Version + 1 : 1;

            // Mark old versions as not current
            if (currentVersion != null)
            {
                currentVersion.IsCurrentVersion = false;
                await _documentRepository.UpdateAsync(currentVersion);
            }

            // Generate file name
            var fileName = $"{taskId}_{nextVersion}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var folderPath = Path.Combine("uploads", "documents", taskId.ToString());

            // Save file using file storage service
            var relativeFilePath = await _fileStorageService.SaveFileAsync(file.OpenReadStream(), fileName, folderPath);

            var document = new Document
            {
                OnboardingTaskId = taskId,
                FileName = fileName,
                OriginalFileName = file.FileName,
                FilePath = relativeFilePath,
                FileSize = file.Length,
                ContentType = file.ContentType,
                UploadedBy = uploadedBy,
                UploadDate = DateTime.UtcNow,
                Version = nextVersion,
                IsCurrentVersion = true,
                Status = DocumentStatus.Pending
            };

            var createdDocument = await _documentRepository.AddAsync(document);
            var documentDto = _mapper.Map<DocumentDto>(createdDocument);

            return new Response<DocumentDto>(documentDto, "Document uploaded successfully.");
        }

        public async Task<Response<DocumentDto>> GetDocumentByIdAsync(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return new Response<DocumentDto>("Document not found.");
            }

            var documentDto = _mapper.Map<DocumentDto>(document);
            return new Response<DocumentDto>(documentDto);
        }

        public async Task<Response<IEnumerable<DocumentDto>>> GetTaskDocumentsAsync(int taskId)
        {
            var documents = await _documentRepository.GetByTaskIdAsync(taskId);
            var documentDtos = _mapper.Map<IEnumerable<DocumentDto>>(documents);

            return new Response<IEnumerable<DocumentDto>>(documentDtos);
        }

        public async Task<Response<byte[]>> GetDocumentFileAsync(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return new Response<byte[]>("Document not found.");
            }

            try
            {
                var fileBytes = await _fileStorageService.GetFileAsync(document.FilePath);
                return new Response<byte[]>(fileBytes);
            }
            catch (FileNotFoundException ex)
            {
                return new Response<byte[]>($"File not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<byte[]>($"Error retrieving file: {ex.Message}");
            }
        }

        public async Task<Response<DocumentDto>> ReviewDocumentAsync(int id, ReviewDocumentDto dto)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return new Response<DocumentDto>("Document not found.");
            }

            // Business rule: Can only review documents with status = Pending
            if (document.Status != DocumentStatus.Pending)
            {
                return new Response<DocumentDto>($"Document cannot be reviewed. Current status: {document.Status}");
            }

            // Business rule: If rejected, comments are mandatory
            if (dto.Status == DocumentStatus.Rejected && string.IsNullOrWhiteSpace(dto.Comments))
            {
                return new Response<DocumentDto>("Comments are required when rejecting a document.");
            }

            document.Status = dto.Status;
            document.ReviewedBy = dto.ReviewedBy;
            document.ReviewedDate = DateTime.UtcNow;
            document.ReviewComments = dto.Comments;
            document.Comments = dto.Comments; // Keep for backward compatibility

            await _documentRepository.UpdateAsync(document);
            var documentDto = _mapper.Map<DocumentDto>(document);

            // Send email notification to employee
            try
            {
                // Get task and employee details
                var task = await _taskRepository.GetByIdAsync(document.OnboardingTaskId);
                var employee = task != null ? await _employeeRepository.GetByIdAsync(task.EmployeeId) : null;
                
                if (employee != null && task != null)
                {
                    var employeeName = $"{employee.FirstName} {employee.LastName}";
                    var taskName = task.TaskTemplate?.Name ?? "Onboarding Task";
                    
                    // Get reviewer name
                    var reviewer = await _userRepository.GetByIdAsync(dto.ReviewedBy);
                    var reviewerName = "HR Team";
                    if (reviewer?.Employee != null)
                    {
                        reviewerName = $"{reviewer.Employee.FirstName} {reviewer.Employee.LastName}";
                    }
                    
                    if (dto.Status == DocumentStatus.Approved)
                    {
                        await _emailService.SendDocumentApprovedEmailAsync(
                            employee.Email,
                            employeeName,
                            document.OriginalFileName,
                            taskName,
                            dto.Comments ?? "",
                            reviewerName
                        );
                    }
                    else if (dto.Status == DocumentStatus.Rejected)
                    {
                        await _emailService.SendDocumentRejectedEmailAsync(
                            employee.Email,
                            employeeName,
                            document.OriginalFileName,
                            taskName,
                            dto.Comments ?? "No comments provided",
                            reviewerName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail document review
                Console.WriteLine($"Failed to send document review email: {ex.Message}");
            }

            var statusMessage = dto.Status == DocumentStatus.Approved ? "approved" : "rejected";
            return new Response<DocumentDto>(documentDto, $"Document {statusMessage} successfully. Notification email sent.");
        }

        public async Task<Response<bool>> DeleteDocumentAsync(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return new Response<bool>("Document not found.");
            }

            await _documentRepository.DeleteAsync(document);
            return new Response<bool>(true, "Document deleted successfully.");
        }
    }
}

