using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using System.Text;

namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class PdfSummaryService : IPdfSummaryService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOnboardingTaskRepository _taskRepository;
        private readonly IDocumentRepository _documentRepository;

        public PdfSummaryService(
            IEmployeeRepository employeeRepository,
            IOnboardingTaskRepository taskRepository,
            IDocumentRepository documentRepository)
        {
            _employeeRepository = employeeRepository;
            _taskRepository = taskRepository;
            _documentRepository = documentRepository;
        }

        public async Task<Response<byte[]>> GenerateOnboardingSummaryAsync(int employeeId)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    return new Response<byte[]>("Employee not found.");
                }

                var tasks = await _taskRepository.GetByEmployeeIdAsync(employeeId);

                // Create Word document
                using var wordDocument = new WordDocument();
                
                // Add section
                IWSection section = wordDocument.AddSection();
                section.PageSetup.Margins.All = 72; // 1 inch margins

                // Add header with company branding
                IWParagraph headerPara = section.AddParagraph();
                headerPara.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                IWTextRange headerText = headerPara.AppendText("EMPLOYEE ONBOARDING SUMMARY");
                headerText.CharacterFormat.FontSize = 18;
                headerText.CharacterFormat.Bold = true;
                headerPara.AppendBreak(BreakType.LineBreak);

                // Add employee information section
                IWParagraph empInfoPara = section.AddParagraph();
                empInfoPara.AppendBreak(BreakType.LineBreak);
                IWTextRange empInfoTitle = empInfoPara.AppendText("Employee Information");
                empInfoTitle.CharacterFormat.FontSize = 14;
                empInfoTitle.CharacterFormat.Bold = true;
                empInfoPara.AppendBreak(BreakType.LineBreak);

                // Employee details
                AddInfoRow(section, "Full Name:", $"{employee.FirstName} {employee.LastName}");
                AddInfoRow(section, "Email:", employee.Email);
                AddInfoRow(section, "Employee Number:", employee.EmployeeNumber);
                AddInfoRow(section, "Department:", employee.Department ?? "N/A");
                AddInfoRow(section, "Position:", employee.Position ?? "N/A");
                AddInfoRow(section, "Hire Date:", employee.HireDate.ToString("yyyy-MM-dd"));
                AddInfoRow(section, "Onboarding Completion Date:", 
                    employee.OnboardingCompletedDate?.ToString("yyyy-MM-dd") ?? "N/A");

                section.AddParagraph().AppendBreak(BreakType.LineBreak);

                // Add task summary section
                IWParagraph taskTitlePara = section.AddParagraph();
                IWTextRange taskTitle = taskTitlePara.AppendText("Task Summary");
                taskTitle.CharacterFormat.FontSize = 14;
                taskTitle.CharacterFormat.Bold = true;
                taskTitlePara.AppendBreak(BreakType.LineBreak);

                // Create table for tasks
                IWTable taskTable = section.AddTable();
                taskTable.ResetCells(tasks.Count() + 1, 5); // Header + rows

                // Table header
                var headerRow = taskTable.Rows[0];
                SetCellText(headerRow.Cells[0], "Task Title", true);
                SetCellText(headerRow.Cells[1], "Status", true);
                SetCellText(headerRow.Cells[2], "Due Date", true);
                SetCellText(headerRow.Cells[3], "Completion Date", true);
                SetCellText(headerRow.Cells[4], "Documents", true);

                // Add task rows
                int rowIndex = 1;
                foreach (var task in tasks.OrderBy(t => t.AssignedDate))
                {
                    var row = taskTable.Rows[rowIndex];
                    SetCellText(row.Cells[0], task.TaskTemplate.Name, false);
                    SetCellText(row.Cells[1], task.Status.ToString(), false);
                    SetCellText(row.Cells[2], task.DueDate.ToString("yyyy-MM-dd"), false);
                    SetCellText(row.Cells[3], task.CompletedDate?.ToString("yyyy-MM-dd") ?? "N/A", false);
                    
                    // Get documents for this task
                    var documents = await _documentRepository.GetByTaskIdAsync(task.Id);
                    var docNames = documents.Select(d => d.OriginalFileName).ToList();
                    SetCellText(row.Cells[4], docNames.Any() ? string.Join(", ", docNames) : "None", false);
                    
                    rowIndex++;
                }

                // Add footer
                section.AddParagraph().AppendBreak(BreakType.LineBreak);
                IWParagraph footerPara = section.AddParagraph();
                footerPara.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                IWTextRange footerText = footerPara.AppendText($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                footerText.CharacterFormat.FontSize = 10;
                footerText.CharacterFormat.Italic = true;

                // Convert Word document to PDF
                using var renderer = new DocIORenderer();
                using var pdfDocument = renderer.ConvertToPDF(wordDocument);
                using var stream = new MemoryStream();
                pdfDocument.Save(stream);
                stream.Position = 0;

                return new Response<byte[]>(stream.ToArray(), "PDF summary generated successfully.");
            }
            catch (Exception ex)
            {
                return new Response<byte[]>($"Error generating PDF summary: {ex.Message}");
            }
        }

        private void AddInfoRow(IWSection section, string label, string value)
        {
            IWParagraph para = section.AddParagraph();
            IWTextRange labelText = para.AppendText(label);
            labelText.CharacterFormat.Bold = true;
            para.AppendText($" {value}");
            para.AppendBreak(BreakType.LineBreak);
        }

        private void SetCellText(Syncfusion.DocIO.DLS.WTableCell cell, string text, bool isBold)
        {
            cell.AddParagraph().AppendText(text).CharacterFormat.Bold = isBold;
        }
    }
}

