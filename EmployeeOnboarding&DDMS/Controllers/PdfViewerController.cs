using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Syncfusion.EJ2.PdfViewer;
using System.Collections.Generic;
using System.IO;
using System;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfViewerController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;
        private readonly string _documentsRoot;
        private readonly IDocumentRepository _documentRepository;

        public PdfViewerController(
            IWebHostEnvironment env, 
            IMemoryCache cache,
            IDocumentRepository documentRepository)
        {
            _env = env;
            _cache = cache;
            _documentRepository = documentRepository;
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _documentsRoot = Path.Combine(webRoot, "uploads", "documents");
        }

        // Core endpoint used by Syncfusion PDF Viewer to load a PDF document
        [HttpPost("load")]
        public IActionResult Load([FromBody] Dictionary<string, object> jsonObject)
        {
            try
            {
                var renderer = new PdfRenderer(_cache);
                var document = GetString(jsonObject, "document");
                var isFileName = GetBool(jsonObject, "isFileName");

                using var stream = new MemoryStream();
                if (!string.IsNullOrWhiteSpace(document))
                {
                    if (isFileName)
                    {
                        var path = ResolveDocumentPath(document);
                        if (!System.IO.File.Exists(path))
                        {
                            return NotFound(new { error = $"File '{document}' not found." });
                        }
                        stream.Write(System.IO.File.ReadAllBytes(path));
                        stream.Position = 0;
                    }
                    else
                    {
                        // base64 document
                        var base64 = document.Contains(",") ? document[(document.IndexOf(',') + 1)..] : document;
                        var bytes = Convert.FromBase64String(base64);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Position = 0;
                    }
                }

                var result = renderer.Load(stream, ToStringDictionary(jsonObject));
                return Content(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("render-pdf-pages")]
        public IActionResult RenderPdfPages([FromBody] Dictionary<string, object> jsonObject)
        {
            var renderer = new PdfRenderer(_cache);
            var result = renderer.GetPage(ToStringDictionary(jsonObject));
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpPost("render-thumbnail-images")]
        public IActionResult RenderThumbnailImages([FromBody] Dictionary<string, object> jsonObject)
        {
            var renderer = new PdfRenderer(_cache);
            var result = renderer.GetThumbnailImages(ToStringDictionary(jsonObject));
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpPost("render-annotation-comments")]
        public IActionResult RenderAnnotationComments([FromBody] Dictionary<string, object> jsonObject)
        {
            var renderer = new PdfRenderer(_cache);
            var result = renderer.GetAnnotationComments(ToStringDictionary(jsonObject));
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpPost("export-annotations")]
        public async Task<IActionResult> ExportAnnotations([FromBody] Dictionary<string, object> jsonObject)
        {
            try
            {
                var renderer = new PdfRenderer(_cache);
                var result = renderer.ExportAnnotation(ToStringDictionary(jsonObject));
                
                // Store annotations in database if documentId is provided
                var documentIdStr = GetString(jsonObject, "documentId");
                if (!string.IsNullOrEmpty(documentIdStr) && int.TryParse(documentIdStr, out var docId))
                {
                    var doc = await _documentRepository.GetByIdAsync(docId);
                    if (doc != null)
                    {
                        // Check if document is read-only (approved)
                        if (doc.Status == DocumentStatus.Approved)
                        {
                            return StatusCode(403, new { error = "This PDF has been approved and is read-only." });
                        }
                        
                        doc.AnnotationsJson = result;
                        await _documentRepository.UpdateAsync(doc);
                    }
                }
                
                return Content(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("import-annotations")]
        public async Task<IActionResult> ImportAnnotations([FromBody] Dictionary<string, object> jsonObject)
        {
            try
            {
                // Check if document is read-only
                var documentIdStr = GetString(jsonObject, "documentId");
                if (!string.IsNullOrEmpty(documentIdStr) && int.TryParse(documentIdStr, out var docId))
                {
                    var doc = await _documentRepository.GetByIdAsync(docId);
                    if (doc != null && doc.Status == DocumentStatus.Approved)
                    {
                        return StatusCode(403, new { error = "This PDF has been approved and is read-only." });
                    }
                    
                    // If annotations exist in database, return them
                    if (!string.IsNullOrWhiteSpace(doc?.AnnotationsJson))
                    {
                        return Content(doc.AnnotationsJson);
                    }
                }
                
                var renderer = new PdfRenderer(_cache);
                var result = renderer.ImportAnnotation(ToStringDictionary(jsonObject));
                return Content(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("render-pdf-texts")]
        public IActionResult RenderPdfTexts([FromBody] Dictionary<string, object> jsonObject)
        {
            var renderer = new PdfRenderer(_cache);
            var result = renderer.GetDocumentText(ToStringDictionary(jsonObject));
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        [HttpPost("bookmarks")]
        public IActionResult Bookmarks([FromBody] Dictionary<string, object> jsonObject)
        {
            var renderer = new PdfRenderer(_cache);
            var result = renderer.GetBookmarks(ToStringDictionary(jsonObject));
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        [HttpPost("print-images")]
        public IActionResult PrintImages([FromBody] Dictionary<string, object> jsonObject)
        {
            var renderer = new PdfRenderer(_cache);
            var result = renderer.GetPrintImage(ToStringDictionary(jsonObject));
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        [HttpPost("download")]
        public IActionResult Download([FromBody] Dictionary<string, object> jsonObject)
        {
            var renderer = new PdfRenderer(_cache);
            var documentBase = renderer.GetDocumentAsBase64(ToStringDictionary(jsonObject));
            return Content(documentBase);
        }

        [HttpPost("unload")]
        public IActionResult Unload([FromBody] Dictionary<string, object> jsonObject)
        {
            var renderer = new PdfRenderer(_cache);
            renderer.ClearCache(ToStringDictionary(jsonObject));
            return Ok(new { message = "Document cache cleared" });
        }

        private string ResolveDocumentPath(string fileName)
        {
            var candidate = Path.Combine(_documentsRoot, fileName);
            if (System.IO.File.Exists(candidate))
            {
                return candidate;
            }

            // fallback to direct path if provided
            return fileName;
        }

        private static Dictionary<string, string> ToStringDictionary(Dictionary<string, object> jsonObject)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in jsonObject)
            {
                dict[kv.Key] = kv.Value?.ToString() ?? string.Empty;
            }
            return dict;
        }

        private static string? GetString(Dictionary<string, object> jsonObject, string key)
        {
            return jsonObject.TryGetValue(key, out var value) ? value?.ToString() : null;
        }

        private static bool GetBool(Dictionary<string, object> jsonObject, string key)
        {
            if (!jsonObject.TryGetValue(key, out var value)) return false;
            if (value is bool b) return b;
            return bool.TryParse(value?.ToString(), out var parsed) && parsed;
        }
    }
}

