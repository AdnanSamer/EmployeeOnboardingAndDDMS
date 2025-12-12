using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EmployeeOnboarding_DDMS.Aplication
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register Services
            services.AddScoped<Interfaces.IAuthService, Services.AuthService>();
            services.AddScoped<Interfaces.IEmployeeService, Services.EmployeeService>();
            services.AddScoped<Interfaces.ITaskTemplateService, Services.TaskTemplateService>();
            services.AddScoped<Interfaces.IOnboardingTaskService, Services.OnboardingTaskService>();
            services.AddScoped<Interfaces.IDocumentService, Services.DocumentService>();
            services.AddScoped<Interfaces.IDashboardService, Services.DashboardService>();
            services.AddScoped<Interfaces.IPdfSummaryService, Services.PdfSummaryService>();
            services.AddScoped<Interfaces.IAdminService, Services.AdminService>();
            services.AddScoped<Interfaces.INotificationService, Services.NotificationService>();
            services.AddScoped<Interfaces.IEmployeeDashboardService, Services.EmployeeDashboardService>();
            services.AddScoped<Interfaces.IOnboardingSummaryService, Services.OnboardingSummaryService>();
        }
    }
}

