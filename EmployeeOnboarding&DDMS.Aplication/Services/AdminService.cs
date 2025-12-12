using System.Text.Json;
using System.Linq;
using TaskStatusEnum = EmployeeOnboarding_DDMS.Domain.Enums.TaskStatus;
using BCrypt.Net;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Admin;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Enums;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOnboardingTaskRepository _taskRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;

        private const string SystemSettingsKey = "SystemSettings";

        private static readonly List<Permission> DefaultPermissions = new()
        {
            new Permission { PermissionName = "ViewEmployees", Description = "View employee list and details" },
            new Permission { PermissionName = "EditTasks", Description = "Create and edit tasks" },
            new Permission { PermissionName = "UploadFiles", Description = "Upload documents" },
            new Permission { PermissionName = "AccessReports", Description = "View reports and analytics" },
            new Permission { PermissionName = "ManageRoles", Description = "Manage user roles and permissions" },
            new Permission { PermissionName = "ManageSystemSettings", Description = "Configure system settings" },
            new Permission { PermissionName = "ManageUsers", Description = "Create, edit, and delete users" }
        };

        public AdminService(
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            ISystemSettingRepository systemSettingRepository,
            IActivityLogRepository activityLogRepository,
            IEmployeeRepository employeeRepository,
            IOnboardingTaskRepository taskRepository,
            IDocumentRepository documentRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _systemSettingRepository = systemSettingRepository;
            _activityLogRepository = activityLogRepository;
            _employeeRepository = employeeRepository;
            _taskRepository = taskRepository;
            _documentRepository = documentRepository;
            _configuration = configuration;
        }

        public async Task<PagedResponse<IEnumerable<AdminUserDto>>> GetUsersAsync(int pageNumber, int pageSize, string? search)
        {
            var (users, total) = await _userRepository.GetPagedAsync(pageNumber, pageSize, search);
            var dto = users.Select(MapUserToDto);
            return new PagedResponse<IEnumerable<AdminUserDto>>(dto, pageNumber, pageSize, total);
        }

        public async Task<Response<AdminUserDto>> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new Response<AdminUserDto>("User not found");
            }
            return new Response<AdminUserDto>(MapUserToDto(user), "User retrieved successfully");
        }

        public async Task<Response<AdminUserDto>> CreateUserAsync(CreateAdminUserDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                return new Response<AdminUserDto>("Email already exists");
            }

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = MapToDomainRole(dto.Role),
                IsActive = true,
                CreatedBy = "Admin",
                LastModifiedBy = "Admin",
                Created = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Create User", "User", user.Id, user.Email);

            return new Response<AdminUserDto>(MapUserToDto(user), "User created successfully");
        }

        public async Task<Response<AdminUserDto>> UpdateUserAsync(int userId, UpdateAdminUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new Response<AdminUserDto>("User not found");
            }

            if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase) &&
                await _userRepository.EmailExistsAsync(dto.Email))
            {
                return new Response<AdminUserDto>("Email already exists");
            }

            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Role = MapToDomainRole(dto.Role);
            user.IsActive = dto.IsActive;
            user.LastModifiedBy = "Admin";
            await _userRepository.UpdateAsync(user);

            await LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Update User", "User", user.Id, user.Email);

            return new Response<AdminUserDto>(MapUserToDto(user), "User updated successfully");
        }

        public async Task<Response<bool>> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new Response<bool>("User not found");
            }

            await _userRepository.DeleteAsync(user);
            await LogAsync(userId, $"{user.FirstName} {user.LastName}", "Delete User", "User", user.Id, user.Email);

            return new Response<bool>(true, "User deleted successfully");
        }

        public async Task<Response<bool>> ResetPasswordAsync(ResetPasswordAdminDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                return new Response<bool>("User not found");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.LastModifiedBy = "Admin";
            await _userRepository.UpdateAsync(user);

            await LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Reset Password", "User", user.Id, user.Email);
            return new Response<bool>(true, "Password reset successfully");
        }

        public async Task<Response<IEnumerable<RoleDto>>> GetRolesAsync()
        {
            await EnsurePermissionsSeededAsync();

            var permissions = await _permissionRepository.GetAllAsync();
            var adminHRPerms = await EnsureRolePermissionsAsync(0, permissions.Select(p => p.Id)); // AdminHR gets all
            var employeePerms = await EnsureRolePermissionsAsync(1, permissions.Where(p => p.PermissionName is "UploadFiles").Select(p => p.Id));

            var roles = new List<RoleDto>
            {
                new RoleDto { RoleId = 0, RoleName = "Admin/HR", Permissions = MapPermissions(adminHRPerms) },
                new RoleDto { RoleId = 1, RoleName = "Employee", Permissions = MapPermissions(employeePerms) }
            };

            return new Response<IEnumerable<RoleDto>>(roles, "Roles retrieved successfully");
        }

        public async Task<Response<IEnumerable<PermissionDto>>> GetPermissionsAsync()
        {
            await EnsurePermissionsSeededAsync();
            var permissions = await _permissionRepository.GetAllAsync();
            var dto = permissions.Select(p => new PermissionDto
            {
                PermissionId = p.Id,
                PermissionName = p.PermissionName,
                Description = p.Description
            });
            return new Response<IEnumerable<PermissionDto>>(dto, "Permissions retrieved successfully");
        }

        public async Task<Response<bool>> AssignPermissionsAsync(AssignPermissionsDto dto)
        {
            await EnsurePermissionsSeededAsync();
            await _permissionRepository.SetRolePermissionsAsync(dto.RoleId, dto.PermissionIds, "Admin");
            await LogAsync(null, "Admin", "Assign Permissions", "Role", dto.RoleId, string.Join(",", dto.PermissionIds));
            return new Response<bool>(true, "Permissions assigned successfully");
        }

        public async Task<Response<SystemSettingsDto>> GetSettingsAsync()
        {
            var existing = await _systemSettingRepository.GetByKeyAsync(SystemSettingsKey);
            if (existing == null)
            {
                return new Response<SystemSettingsDto>(BuildDefaultSettings(), "Settings retrieved successfully (defaults)");
            }

            var settings = JsonSerializer.Deserialize<SystemSettingsDto>(existing.SettingValue) ?? BuildDefaultSettings();
            return new Response<SystemSettingsDto>(settings, "Settings retrieved successfully");
        }

        public async Task<Response<SystemSettingsDto>> UpdateSettingsAsync(SystemSettingsDto dto, string updatedBy)
        {
            var json = JsonSerializer.Serialize(dto);
            await _systemSettingRepository.UpsertAsync(SystemSettingsKey, json, updatedBy);
            await LogAsync(null, updatedBy, "Update System Settings", "Settings", null, null);
            return new Response<SystemSettingsDto>(dto, "Settings updated successfully");
        }

        public async Task<PagedResponse<IEnumerable<ActivityLogDto>>> GetActivityLogsAsync(ActivityLogFilterDto filter)
        {
            var (logs, total) = await _activityLogRepository.GetPagedAsync(
                filter.UserId,
                filter.Action,
                filter.EntityType,
                filter.StartDate,
                filter.EndDate,
                filter.PageNumber,
                filter.PageSize);

            var dto = logs.Select(l => new ActivityLogDto
            {
                LogId = l.Id,
                UserId = l.UserId,
                UserName = l.UserName,
                Action = l.Action,
                EntityType = l.EntityType,
                EntityId = l.EntityId,
                EntityName = l.EntityName,
                Details = l.Details,
                IpAddress = l.IpAddress,
                Timestamp = l.Timestamp
            });

            return new PagedResponse<IEnumerable<ActivityLogDto>>(dto, filter.PageNumber, filter.PageSize, total);
        }

        public async Task<Response<ChartDataDto>> GetOnboardingProgressChartAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            var tasks = await _taskRepository.GetAllAsync();
            var taskLookup = tasks.GroupBy(t => t.EmployeeId).ToDictionary(g => g.Key, g => g.ToList());

            var labels = new List<string>();
            var data = new List<double>();

            foreach (var employee in employees)
            {
                var employeeTasks = taskLookup.ContainsKey(employee.Id) ? taskLookup[employee.Id] : new List<Domain.Entities.OnboardingTask>();
                var total = employeeTasks.Count;
                var completed = employeeTasks.Count(t => t.Status == TaskStatusEnum.Completed);
                var percentage = total > 0 ? (double)completed / total * 100 : 0;

                labels.Add($"{employee.FirstName} {employee.LastName}");
                data.Add(Math.Round(percentage, 2));
            }

            var chart = new ChartDataDto
            {
                Labels = labels,
                Datasets = new[]
                {
                    new ChartDatasetDto
                    {
                        Label = "Completion Percentage",
                        Data = data,
                        BackgroundColor = labels.Select(_ => "#42A5F5").ToList(),
                        BorderColor = labels.Select(_ => "#1E88E5").ToList()
                    }
                }
            };

            return new Response<ChartDataDto>(chart, "Chart data retrieved successfully");
        }

        public async Task<Response<ChartDataDto>> GetTaskCompletionChartAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            var completed = tasks.Count(t => t.Status == TaskStatusEnum.Completed);
            var pending = tasks.Count(t => t.Status == TaskStatusEnum.Pending || t.Status == TaskStatusEnum.InProgress);
            var overdue = tasks.Count(t => (t.Status == TaskStatusEnum.Pending || t.Status == TaskStatusEnum.InProgress) && t.DueDate < DateTime.UtcNow);

            var chart = new ChartDataDto
            {
                Labels = new[] { "Completed", "Pending", "Overdue" },
                Datasets = new[]
                {
                    new ChartDatasetDto
                    {
                        Label = "Tasks",
                        Data = new double[] { completed, pending, overdue },
                        BackgroundColor = new[] { "#66BB6A", "#FFA726", "#EF5350" },
                        BorderColor = new[] { "#43A047", "#FB8C00", "#E53935" }
                    }
                }
            };

            return new Response<ChartDataDto>(chart, "Chart data retrieved successfully");
        }

        public async Task<Response<ChartDataDto>> GetFileUploadsChartAsync()
        {
            var documents = await _documentRepository.GetAllAsync();
            var lastSixMonths = Enumerable.Range(0, 6)
                .Select(i => DateTime.UtcNow.AddMonths(-i))
                .Select(d => new DateTime(d.Year, d.Month, 1))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            var labels = lastSixMonths.Select(d => d.ToString("MMM")).ToList();
            var data = new List<double>();

            foreach (var monthStart in lastSixMonths)
            {
                var monthEnd = monthStart.AddMonths(1);
                var count = documents.Count(d => d.UploadDate >= monthStart && d.UploadDate < monthEnd);
                data.Add(count);
            }

            var chart = new ChartDataDto
            {
                Labels = labels,
                Datasets = new[]
                {
                    new ChartDatasetDto
                    {
                        Label = "File Uploads",
                        Data = data,
                        BackgroundColor = new[] { "#42A5F5" },
                        BorderColor = new[] { "#2196F3" }
                    }
                }
            };

            return new Response<ChartDataDto>(chart, "Chart data retrieved successfully");
        }

        public async Task<Response<IEnumerable<OverdueTaskAlertDto>>> GetOverdueTasksAlertAsync()
        {
            var overdueTasks = await _taskRepository.GetOverdueTasksAsync();
            var alerts = overdueTasks.Select(t => new OverdueTaskAlertDto
            {
                TaskId = t.Id,
                TaskTitle = t.TaskTemplate?.Name ?? $"Task {t.Id}",
                EmployeeId = t.EmployeeId,
                EmployeeName = $"{t.Employee?.FirstName} {t.Employee?.LastName}".Trim(),
                DueDate = t.DueDate,
                Priority = 1,
                DaysOverdue = (int)(DateTime.UtcNow.Date - t.DueDate.Date).TotalDays
            }).ToList();

            return new Response<IEnumerable<OverdueTaskAlertDto>>(alerts, "Overdue tasks retrieved successfully");
        }

        public async Task<Response<IEnumerable<RejectedDocumentAlertDto>>> GetRejectedDocumentsAlertAsync()
        {
            var documents = await _documentRepository.GetAllAsync();
            var rejectedDocs = documents.Where(d => d.Status == DocumentStatus.Rejected).ToList();
            var tasks = await _taskRepository.GetAllAsync();
            var employees = await _employeeRepository.GetAllAsync();

            var taskLookup = tasks.ToDictionary(t => t.Id, t => t.EmployeeId);
            var employeeLookup = employees.ToDictionary(e => e.Id, e => e);

            var alerts = new List<RejectedDocumentAlertDto>();
            foreach (var doc in rejectedDocs)
            {
                var employeeName = string.Empty;
                if (taskLookup.TryGetValue(doc.OnboardingTaskId, out var empId) && employeeLookup.TryGetValue(empId, out var employee))
                {
                    employeeName = $"{employee.FirstName} {employee.LastName}";
                }

                alerts.Add(new RejectedDocumentAlertDto
                {
                    DocumentId = doc.Id,
                    FileName = doc.FileName,
                    EmployeeId = taskLookup.TryGetValue(doc.OnboardingTaskId, out var eid) ? eid : 0,
                    EmployeeName = employeeName,
                    UploadedDate = doc.UploadDate,
                    RejectionReason = doc.ReviewComments ?? "Rejected",
                    ReviewedBy = doc.ReviewedBy?.ToString(),
                    ReviewedDate = doc.ReviewedDate
                });
            }

            return new Response<IEnumerable<RejectedDocumentAlertDto>>(alerts, "Rejected documents retrieved successfully");
        }

        private async Task<IEnumerable<RolePermission>> EnsureRolePermissionsAsync(int roleId, IEnumerable<int> targetPermissionIds)
        {
            var current = await _permissionRepository.GetRolePermissionsAsync(roleId);
            var currentIds = current.Select(rp => rp.PermissionId).ToHashSet();
            var desired = targetPermissionIds.ToHashSet();

            if (!desired.SetEquals(currentIds))
            {
                await _permissionRepository.SetRolePermissionsAsync(roleId, desired, "System");
                current = await _permissionRepository.GetRolePermissionsAsync(roleId);
            }

            return current;
        }

        private async Task EnsurePermissionsSeededAsync()
        {
            await _permissionRepository.EnsurePermissionsSeededAsync(DefaultPermissions, "System");
        }

        private AdminUserDto MapUserToDto(User user)
        {
            return new AdminUserDto
            {
                UserId = user.Id,
                EmployeeId = user.Employee?.Id,  // Include EmployeeId if Employee relationship exists
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = MapToSpecRole(user.Role),
                IsActive = user.IsActive,
                CreatedAt = user.Created,
                LastLogin = user.LastLogin
            };
        }

        private static int MapToSpecRole(UserRole role) => role switch
        {
            UserRole.AdminHR => 0,  // Admin/HR unified
            UserRole.Employee => 1,
            _ => 1
        };

        private static UserRole MapToDomainRole(int roleId) => roleId switch
        {
            0 => UserRole.AdminHR,  // Admin/HR unified
            1 => UserRole.Employee,
            _ => UserRole.Employee
        };

        private IEnumerable<PermissionDto> MapPermissions(IEnumerable<RolePermission> rolePermissions)
        {
            return rolePermissions.Select(rp => new PermissionDto
            {
                PermissionId = rp.Permission.Id,
                PermissionName = rp.Permission.PermissionName,
                Description = rp.Permission.Description
            });
        }

        private async Task LogAsync(int? userId, string userName, string action, string entityType, int? entityId, string? details)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                EntityName = details,
                Details = details,
                Timestamp = DateTime.UtcNow,
                CreatedBy = userName,
                LastModifiedBy = userName
            };
            await _activityLogRepository.AddAsync(log);
        }

        private SystemSettingsDto BuildDefaultSettings()
        {
            var settings = new SystemSettingsDto();
            var smtpSection = _configuration.GetSection("Smtp");
            if (smtpSection.Exists())
            {
                settings.NotificationSettings.SmtpServer = smtpSection["Host"] ?? settings.NotificationSettings.SmtpServer;
                if (int.TryParse(smtpSection["Port"], out var port))
                {
                    settings.NotificationSettings.SmtpPort = port;
                }
                settings.NotificationSettings.EnableSsl = string.Equals(smtpSection["UseSsl"], "true", StringComparison.OrdinalIgnoreCase);
                settings.NotificationSettings.SmtpUsername = smtpSection["UserName"] ?? settings.NotificationSettings.SmtpUsername;
                settings.NotificationSettings.SmtpPassword = smtpSection["Password"] ?? settings.NotificationSettings.SmtpPassword;
                settings.NotificationSettings.SmtpFromEmail = smtpSection["From"] ?? settings.NotificationSettings.SmtpFromEmail;
                settings.NotificationSettings.SmtpFromName = smtpSection["From"] ?? settings.NotificationSettings.SmtpFromName;
            }
            return settings;
        }
    }
}

