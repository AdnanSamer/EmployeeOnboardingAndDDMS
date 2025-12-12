using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Infrastructure.Contexts;
using EmployeeOnboarding_DDMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeOnboarding_DDMS.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Register repositories
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<ITaskTemplateRepository, TaskTemplateRepository>();
            services.AddTransient<IOnboardingTaskRepository, OnboardingTaskRepository>();
            services.AddTransient<IDocumentRepository, DocumentRepository>();
            services.AddTransient<IOnboardingSummaryRepository, OnboardingSummaryRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<IActivityLogRepository, ActivityLogRepository>();
            services.AddTransient<ISystemSettingRepository, SystemSettingRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();

            // Register infrastructure services
            services.AddScoped<Aplication.Interfaces.IFileStorageService, Infrastructure.Services.FileStorageService>();
            services.AddScoped<Aplication.Interfaces.IEmailService, Infrastructure.Services.EmailService>();
        }
    }
}
