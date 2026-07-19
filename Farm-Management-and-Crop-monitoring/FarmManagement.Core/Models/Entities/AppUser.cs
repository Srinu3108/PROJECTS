using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.Entities;

public class AppUser
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Agronomist;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string PasswordHint { get; set; } = string.Empty;
    public bool IsBlocked { get; set; } = false;
}
