using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding_DDMS.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PermissionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Permission>> GetAllAsync()
        {
            return await _dbContext.Permissions.ToListAsync();
        }

        public async Task<IReadOnlyList<RolePermission>> GetRolePermissionsAsync(int roleId)
        {
            return await _dbContext.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.Role == roleId)
                .ToListAsync();
        }

        public async Task SetRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds, string updatedBy = "System")
        {
            var existing = _dbContext.RolePermissions.Where(rp => rp.Role == roleId);
            _dbContext.RolePermissions.RemoveRange(existing);

            var newAssignments = permissionIds.Select(pid => new RolePermission
            {
                Role = roleId,
                PermissionId = pid,
                CreatedBy = updatedBy,
                LastModifiedBy = updatedBy
            });

            await _dbContext.RolePermissions.AddRangeAsync(newAssignments);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EnsurePermissionsSeededAsync(IEnumerable<Permission> permissions, string createdBy = "System")
        {
            var existingNames = await _dbContext.Permissions.Select(p => p.PermissionName).ToListAsync();
            var toAdd = permissions.Where(p => !existingNames.Contains(p.PermissionName, StringComparer.OrdinalIgnoreCase))
                .Select(p => new Permission
                {
                    PermissionName = p.PermissionName,
                    Description = p.Description,
                    CreatedBy = createdBy,
                    LastModifiedBy = createdBy
                });

            if (toAdd.Any())
            {
                await _dbContext.Permissions.AddRangeAsync(toAdd);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}

