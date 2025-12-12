using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EmployeeOnboarding_DDMS.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly bool _useSsl;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpHost = _configuration["Smtp:Host"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "465");
            _useSsl = bool.Parse(_configuration["Smtp:UseSsl"] ?? "true");
            _userName = _configuration["Smtp:UserName"] ?? "";
            _password = _configuration["Smtp:Password"] ?? "";
            _fromEmail = _configuration["Smtp:From"] ?? "";
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Employee Onboarding System", _fromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                // For Gmail with port 465, use SSL on connect
                await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_userName, _password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log error but don't throw to prevent breaking the application
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }

        public async Task SendWelcomeEmailAsync(string employeeEmail, string employeeName, string defaultPassword)
        {
            var subject = "Welcome to the Employee Onboarding System";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <h2 style='color: #667eea;'>Welcome to Employee Onboarding!</h2>
                        
                        <p>Hello <strong>{employeeName}</strong>,</p>
                        
                        <p>Your account has been created successfully.</p>
                        
                        <p>You can now log in using the following credentials:</p>
                        
                        <div style='background: #f0f9ff; border-left: 4px solid #3b82f6; padding: 15px; margin: 20px 0;'>
                            <p style='margin: 0;'><strong>Email:</strong> {employeeEmail}</p>
                            <p style='margin: 10px 0 0 0;'><strong>Password:</strong> <code style='background: #e0e0e0; padding: 2px 6px; border-radius: 3px;'>{defaultPassword}</code></p>
                        </div>
                        
                        <p>
                            <a href='https://localhost:4200/login' 
                               style='display: inline-block; background: #667eea; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 10px 0;'>
                                Login Here
                            </a>
                        </p>
                        
                        <p>Thank you,<br><strong>HR Team</strong></p>
                        
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                        
                        <p style='font-size: 12px; color: #999;'>
                            This is an automated message. Please do not reply to this email.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendTaskAssignedEmailAsync(string employeeEmail, string employeeName, string taskName, DateTime dueDate)
        {
            await SendTaskAssignedEmailAsync(employeeEmail, employeeName, taskName, "", dueDate, 1); // Default: no description, medium priority
        }

        public async Task SendTaskAssignedEmailAsync(string employeeEmail, string employeeName, string taskName, string description, DateTime dueDate, int priority)
        {
            var subject = $"New Task Assigned: {taskName}";
            
            var priorityColor = priority switch
            {
                0 => "#10b981", // Low - Green
                2 => "#ef4444", // High - Red
                _ => "#f59e0b"  // Medium - Orange
            };
            
            var priorityText = priority switch
            {
                0 => "Low",
                2 => "High",
                _ => "Medium"
            };
            
            var formattedDueDate = dueDate.ToString("MMMM dd, yyyy");
            var descriptionHtml = string.IsNullOrWhiteSpace(description) 
                ? "" 
                : $"<p style='margin: 5px 0;'><strong>Description:</strong> {description}</p>";
            
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <h2 style='color: #667eea;'>New Task Assigned</h2>
                        
                        <p>Hello <strong>{employeeName}</strong>,</p>
                        
                        <p>A new task has been assigned to you.</p>
                        
                        <div style='background: #f5f5f5; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                            <h3 style='margin: 0 0 10px 0; color: #333;'>{taskName}</h3>
                            {descriptionHtml}
                            <p style='margin: 5px 0;'><strong>Due Date:</strong> {formattedDueDate}</p>
                            <p style='margin: 5px 0;'><strong>Priority:</strong> <span style='color: {priorityColor}; font-weight: bold;'>{priorityText}</span></p>
                        </div>
                        
                        <p>
                            <a href='https://localhost:4200/my-tasks' 
                               style='display: inline-block; background: #667eea; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 10px 0;'>
                                View Task
                            </a>
                        </p>
                        
                        <p>Please complete this task by the due date.</p>
                        
                        <p>Thank you,<br><strong>HR Team</strong></p>
                        
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                        
                        <p style='font-size: 12px; color: #999;'>
                            This is an automated notification. Please do not reply to this email.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendTaskDueSoonEmailAsync(string employeeEmail, string employeeName, string taskName, DateTime dueDate, int daysRemaining)
        {
            var subject = "Task Due Soon - Action Required";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #f59e0b;'>Task Due Soon</h2>
                    <p>Hello {employeeName},</p>
                    <p>This is a reminder that the following task is due soon:</p>
                    <div style='background-color: #fef3c7; padding: 15px; border-left: 4px solid #f59e0b; margin: 20px 0;'>
                        <strong>Task:</strong> {taskName}<br>
                        <strong>Due Date:</strong> {dueDate:MMMM dd, yyyy}<br>
                        <strong>Days Remaining:</strong> {daysRemaining} day(s)
                    </div>
                    <p>Please complete this task as soon as possible to stay on track with your onboarding.</p>
                    <p>Best regards,<br>HR Team</p>
                </body>
                </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendTaskOverdueEmailAsync(string employeeEmail, string employeeName, string taskName, DateTime dueDate, int daysOverdue)
        {
            var subject = "⚠️ Task Overdue - Immediate Action Required";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #dc2626;'>Task Overdue</h2>
                    <p>Hello {employeeName},</p>
                    <p>The following task is now overdue and requires immediate attention:</p>
                    <div style='background-color: #fee2e2; padding: 15px; border-left: 4px solid #dc2626; margin: 20px 0;'>
                        <strong>Task:</strong> {taskName}<br>
                        <strong>Due Date:</strong> {dueDate:MMMM dd, yyyy}<br>
                        <strong>Days Overdue:</strong> {daysOverdue} day(s)
                    </div>
                    <p>Please complete this task immediately. If you need assistance, contact your HR representative.</p>
                    <p>Best regards,<br>HR Team</p>
                </body>
                </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendDocumentApprovedEmailAsync(string employeeEmail, string employeeName, string documentName, string taskName)
        {
            await SendDocumentApprovedEmailAsync(employeeEmail, employeeName, documentName, taskName, "", "HR Team");
        }

        public async Task SendDocumentApprovedEmailAsync(string employeeEmail, string employeeName, string documentName, string taskName, string comments, string reviewerName)
        {
            var subject = $"Document Approved: {documentName}";
            
            var commentsSection = string.IsNullOrWhiteSpace(comments) ? "" : $@"
                <div style='background: #f0f9ff; border-left: 4px solid #3b82f6; padding: 15px; margin: 20px 0;'>
                    <p style='margin: 0 0 5px 0;'><strong>Reviewer Comments:</strong></p>
                    <p style='margin: 0; color: #1e40af;'>{comments}</p>
                </div>
            ";
            
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <div style='background: #10b981; color: white; padding: 15px; border-radius: 8px 8px 0 0; margin: -20px -20px 20px -20px;'>
                            <h2 style='margin: 0;'>✓ Document Approved</h2>
                        </div>
                        
                        <p>Hello <strong>{employeeName}</strong>,</p>
                        
                        <p>Your document has been reviewed and <strong style='color: #10b981;'>approved</strong>.</p>
                        
                        <div style='background: #f0fdf4; border-left: 4px solid #10b981; padding: 15px; margin: 20px 0;'>
                            <p style='margin: 0 0 10px 0;'><strong>Document:</strong> {documentName}</p>
                            <p style='margin: 0 0 10px 0;'><strong>Task:</strong> {taskName}</p>
                            <p style='margin: 0 0 10px 0;'><strong>Status:</strong> <span style='color: #10b981;'>Approved</span></p>
                            <p style='margin: 0;'><strong>Reviewed By:</strong> {reviewerName}</p>
                        </div>
                        
                        {commentsSection}
                        
                        <p>
                            <a href='https://localhost:4200/my-documents' 
                               style='display: inline-block; background: #10b981; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 10px 0;'>
                                View Documents
                            </a>
                        </p>
                        
                        <p>Great job! You can now proceed with the next steps in your onboarding.</p>
                        
                        <p>Thank you,<br><strong>HR Team</strong></p>
                        
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                        
                        <p style='font-size: 12px; color: #999;'>
                            This is an automated notification. Please do not reply to this email.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendDocumentRejectedEmailAsync(string employeeEmail, string employeeName, string documentName, string taskName, string reason)
        {
            await SendDocumentRejectedEmailAsync(employeeEmail, employeeName, documentName, taskName, reason, "HR Team");
        }

        public async Task SendDocumentRejectedEmailAsync(string employeeEmail, string employeeName, string documentName, string taskName, string reason, string reviewerName)
        {
            var subject = $"Document Rejected: {documentName} - Action Required";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <div style='background: #ef4444; color: white; padding: 15px; border-radius: 8px 8px 0 0; margin: -20px -20px 20px -20px;'>
                            <h2 style='margin: 0;'>✗ Document Rejected</h2>
                        </div>
                        
                        <p>Hello <strong>{employeeName}</strong>,</p>
                        
                        <p>Your document has been reviewed and <strong style='color: #ef4444;'>requires corrections</strong>.</p>
                        
                        <div style='background: #fef2f2; border-left: 4px solid #ef4444; padding: 15px; margin: 20px 0;'>
                            <p style='margin: 0 0 10px 0;'><strong>Document:</strong> {documentName}</p>
                            <p style='margin: 0 0 10px 0;'><strong>Task:</strong> {taskName}</p>
                            <p style='margin: 0 0 10px 0;'><strong>Status:</strong> <span style='color: #ef4444;'>Rejected</span></p>
                            <p style='margin: 0;'><strong>Reviewed By:</strong> {reviewerName}</p>
                        </div>
                        
                        <div style='background: #fff7ed; border-left: 4px solid #f59e0b; padding: 15px; margin: 20px 0;'>
                            <p style='margin: 0 0 5px 0;'><strong>Review Comments:</strong></p>
                            <p style='margin: 0; color: #92400e;'>{reason}</p>
                        </div>
                        
                        <p>
                            <a href='https://localhost:4200/my-documents' 
                               style='display: inline-block; background: #ef4444; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 10px 0;'>
                                Upload Corrected Document
                            </a>
                        </p>
                        
                        <p>Please review the comments above, make the necessary corrections, and re-upload your document.</p>
                        
                        <p>Thank you,<br><strong>HR Team</strong></p>
                        
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                        
                        <p style='font-size: 12px; color: #999;'>
                            This is an automated notification. Please do not reply to this email.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendOnboardingCompletedEmailAsync(string employeeEmail, string employeeName, DateTime completionDate)
        {
            var subject = "🎉 Congratulations! Onboarding Completed";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #16a34a;'>Onboarding Completed!</h2>
                    <p>Hello {employeeName},</p>
                    <p>Congratulations! You have successfully completed all your onboarding tasks.</p>
                    <div style='background-color: #dcfce7; padding: 15px; border-left: 4px solid #16a34a; margin: 20px 0;'>
                        <strong>Completion Date:</strong> {completionDate:MMMM dd, yyyy}<br>
                        <strong>Status:</strong> All tasks completed ✓
                    </div>
                    <p>Welcome to the team! We're excited to have you on board.</p>
                    <p>If you have any questions, please don't hesitate to reach out to your manager or HR.</p>
                    <p>Best regards,<br>HR Team</p>
                </body>
                </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }
    }
}
