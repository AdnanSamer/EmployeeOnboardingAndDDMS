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
            var subject = "Welcome to the Team - Your Account Details";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #2563eb;'>Welcome to the Team!</h2>
                    <p>Hello {employeeName},</p>
                    <p>Welcome aboard! We're excited to have you join our team. Your employee account has been created.</p>
                    <div style='background-color: #f3f4f6; padding: 15px; border-left: 4px solid #2563eb; margin: 20px 0;'>
                        <strong>Login Credentials:</strong><br>
                        <strong>Email:</strong> {employeeEmail}<br>
                        <strong>Temporary Password:</strong> {defaultPassword}
                    </div>
                    <div style='background-color: #fef3c7; padding: 15px; border-left: 4px solid #f59e0b; margin: 20px 0;'>
                        <strong>⚠️ Important:</strong> You will be required to change your password on first login for security purposes.
                    </div>
                    <p><strong>Next Steps:</strong></p>
                    <ol>
                        <li>Log in to the Employee Portal using the credentials above</li>
                        <li>Change your password to something secure and memorable</li>
                        <li>Complete your onboarding tasks</li>
                        <li>Upload any required documents</li>
                    </ol>
                    <p>If you have any questions or need assistance, please don't hesitate to contact HR.</p>
                    <p>Best regards,<br>HR Team</p>
                </body>
                </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendTaskAssignedEmailAsync(string employeeEmail, string employeeName, string taskName, DateTime dueDate)
        {
            var subject = "New Task Assigned - Employee Onboarding";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #2563eb;'>New Task Assigned</h2>
                    <p>Hello {employeeName},</p>
                    <p>A new task has been assigned to you as part of your onboarding process:</p>
                    <div style='background-color: #f3f4f6; padding: 15px; border-left: 4px solid #2563eb; margin: 20px 0;'>
                        <strong>Task:</strong> {taskName}<br>
                        <strong>Due Date:</strong> {dueDate:MMMM dd, yyyy}
                    </div>
                    <p>Please log in to the Employee Portal to view the task details and complete it before the due date.</p>
                    <p>Best regards,<br>HR Team</p>
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
            var subject = "✅ Document Approved";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #16a34a;'>Document Approved</h2>
                    <p>Hello {employeeName},</p>
                    <p>Great news! Your document has been approved:</p>
                    <div style='background-color: #dcfce7; padding: 15px; border-left: 4px solid #16a34a; margin: 20px 0;'>
                        <strong>Document:</strong> {documentName}<br>
                        <strong>Task:</strong> {taskName}<br>
                        <strong>Status:</strong> Approved ✓
                    </div>
                    <p>You can now proceed with the next steps in your onboarding process.</p>
                    <p>Best regards,<br>HR Team</p>
                </body>
                </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendDocumentRejectedEmailAsync(string employeeEmail, string employeeName, string documentName, string taskName, string reason)
        {
            var subject = "Document Rejected - Action Required";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #dc2626;'>Document Rejected</h2>
                    <p>Hello {employeeName},</p>
                    <p>Your document has been reviewed and requires resubmission:</p>
                    <div style='background-color: #fee2e2; padding: 15px; border-left: 4px solid #dc2626; margin: 20px 0;'>
                        <strong>Document:</strong> {documentName}<br>
                        <strong>Task:</strong> {taskName}<br>
                        <strong>Status:</strong> Rejected<br>
                        <strong>Reason:</strong> {reason}
                    </div>
                    <p>Please review the feedback and upload a corrected version of the document.</p>
                    <p>Best regards,<br>HR Team</p>
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
