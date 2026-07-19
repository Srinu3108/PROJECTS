using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class AccountService : IAccountService
{
    private readonly FarmDbContext _db;
    public AccountService(FarmDbContext db) => _db = db;

    public async Task<AppUser?> AuthenticateAsync(string email, string password)
    {
        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == email.ToLower());
        if (user == null) return null;
        return PasswordHelper.Verify(password, user.PasswordHash) ? user : null;
    }

    public async Task<bool> RegisterAsync(RegisterViewModel vm)
    {
        if (await EmailExistsAsync(vm.Email)) return false;

        _db.AppUsers.Add(new AppUser
        {
            FullName = vm.FullName,
            Email = vm.Email.ToLower(),
            PasswordHash = PasswordHelper.Hash(vm.Password),
            Role = vm.Role,
            CreatedAt = DateTime.Now,
            PasswordHint = vm.PasswordHint
        });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email) =>
        await _db.AppUsers.AnyAsync(u => u.Email == email.ToLower());

    public async Task<string?> GetPasswordHintAsync(string email)
    {
        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == email.ToLower());
        return user?.PasswordHint;
    }

    public async Task<bool> ResetPasswordAsync(string email, string hint, string newPassword)
    {
        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == email.ToLower());
        if (user == null || !string.Equals(user.PasswordHint, hint, StringComparison.OrdinalIgnoreCase))
            return false;

        user.PasswordHash = PasswordHelper.Hash(newPassword);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetAdminCountAsync() =>
        await _db.AppUsers.CountAsync(u => u.Role == Models.Enums.UserRole.Admin);
}
