using EmployeeOnboarding_DDMS.Domain.Common;
using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeOnboarding_DDMS.Infrastructure.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<TaskTemplate> TaskTemplates { get; set; }
        public DbSet<OnboardingTask> OnboardingTasks { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<OnboardingSummary> OnboardingSummaries { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new TaskTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new OnboardingTaskConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentConfiguration());
            modelBuilder.ApplyConfiguration(new OnboardingSummaryConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new EmailLogConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityLogConfiguration());
            modelBuilder.ApplyConfiguration(new SystemSettingConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entry.Entity.Created == default(DateTime))
                        {
                            entry.Entity.Created = DateTime.UtcNow;
                        }
                        if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                        {
                            entry.Entity.CreatedBy = "System";
                        }
                        if (string.IsNullOrEmpty(entry.Entity.LastModifiedBy))
                        {
                            entry.Entity.LastModifiedBy = "System";
                        }
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.UtcNow;
                        if (string.IsNullOrEmpty(entry.Entity.LastModifiedBy))
                        {
                            entry.Entity.LastModifiedBy = "System";
                        }
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entry.Entity.Created == default(DateTime))
                        {
                            entry.Entity.Created = DateTime.UtcNow;
                        }
                        if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                        {
                            entry.Entity.CreatedBy = "System";
                        }
                        if (string.IsNullOrEmpty(entry.Entity.LastModifiedBy))
                        {
                            entry.Entity.LastModifiedBy = "System";
                        }
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.UtcNow;
                        if (string.IsNullOrEmpty(entry.Entity.LastModifiedBy))
                        {
                            entry.Entity.LastModifiedBy = "System";
                        }
                        break;
                }
            }
            return base.SaveChanges();
        }
    }
}
