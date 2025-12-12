using EmployeeOnboarding_DDMS.Domain.Entities;

namespace EmployeeOnboarding_DDMS.Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task<IReadOnlyList<Permission>> GetAllAsync();
        Task<IReadOnlyList<RolePermission>> GetRolePermissionsAsync(int roleId);
        Task SetRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds, string updatedBy = "System");
        Task EnsurePermissionsSeededAsync(IEnumerable<Permission> permissions, string createdBy = "System");
    }
}

