using EmployeeOnboarding_DDMS.Aplication.DTOs.Documents;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("upload/{taskId}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Response<DocumentDto>>> UploadDocument(
            int taskId, 
            [FromForm] UploadDocumentRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new Response<DocumentDto>("File is required."));
            }

            var result = await _documentService.UploadDocumentAsync(taskId, request.File, request.UploadedBy);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<DocumentDto>>> GetDocument(int id)
        {
            var result = await _documentService.GetDocumentByIdAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<Response<IEnumerable<DocumentDto>>>> GetTaskDocuments(int taskId)
        {
            var result = await _documentService.GetTaskDocumentsAsync(taskId);
            return Ok(result);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var result = await _documentService.GetDocumentFileAsync(id);
            if (!result.Succeeded || result.Data == null)
            {
                return NotFound(result);
            }

            var document = await _documentService.GetDocumentByIdAsync(id);
            if (!document.Succeeded || document.Data == null)
            {
                return NotFound(document);
            }

            return File(result.Data, document.Data.ContentType, document.Data.OriginalFileName);
        }

        [HttpGet("{id}/preview")]
        public async Task<IActionResult> PreviewDocument(int id)
        {
            var result = await _documentService.GetDocumentFileAsync(id);
            if (!result.Succeeded || result.Data == null)
            {
                return NotFound(result);
            }

            return File(result.Data, "application/pdf");
        }

        [HttpPut("{id}/review")]
        public async Task<ActionResult<Response<DocumentDto>>> ReviewDocument(int id, [FromBody] ReviewDocumentDto dto)
        {
            dto.DocumentId = id;
            var result = await _documentService.ReviewDocumentAsync(id, dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<bool>>> DeleteDocument(int id)
        {
            var result = await _documentService.DeleteDocumentAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("generate-summary/{employeeId}")]
        public async Task<IActionResult> GenerateSummary(int employeeId, [FromServices] Aplication.Interfaces.IPdfSummaryService pdfSummaryService)
        {
            var result = await pdfSummaryService.GenerateOnboardingSummaryAsync(employeeId);
            if (!result.Succeeded || result.Data == null)
            {
                return BadRequest(result);
            }

            return File(result.Data, "application/pdf", $"OnboardingSummary_{employeeId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
        }
    }
}
