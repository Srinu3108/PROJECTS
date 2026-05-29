using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.Interfaces;

public interface IUserManagementService
{
    Task<IEnumerable<AppUser>> GetAllUsersAsync();
    Task<AppUser?> GetByIdAsync(int userId);
    Task UpdateRoleAsync(int userId, UserRole newRole);
    Task DeleteUserAsync(int userId);
    Task ToggleBlockAsync(int userId);
}
