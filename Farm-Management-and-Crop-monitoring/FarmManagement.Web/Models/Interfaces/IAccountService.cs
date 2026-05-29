using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Interfaces;

public interface IAccountService
{
    Task<AppUser?> AuthenticateAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterViewModel vm);
    Task<bool> EmailExistsAsync(string email);
    Task<string?> GetPasswordHintAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string hint, string newPassword);
    Task<int> GetAdminCountAsync();
}
