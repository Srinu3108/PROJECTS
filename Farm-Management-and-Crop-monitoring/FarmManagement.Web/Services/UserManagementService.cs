using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class UserManagementService : IUserManagementService
{
    private readonly FarmDbContext _db;

    public UserManagementService(FarmDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        => await _db.AppUsers.AsNoTracking().OrderBy(u => u.CreatedAt).ToListAsync();

    public async Task<AppUser?> GetByIdAsync(int userId)
        => await _db.AppUsers.FindAsync(userId);

    public async Task UpdateRoleAsync(int userId, UserRole newRole)
    {
        var user = await _db.AppUsers.FindAsync(userId);
        if (user == null) return;
        user.Role = newRole;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _db.AppUsers.FindAsync(userId);
        if (user == null) return;
        _db.AppUsers.Remove(user);
        await _db.SaveChangesAsync();
    }

    public async Task ToggleBlockAsync(int userId)
    {
        var user = await _db.AppUsers.FindAsync(userId);
        if (user != null)
        {
            user.IsBlocked = !user.IsBlocked;
            await _db.SaveChangesAsync();
        }
    }
}
