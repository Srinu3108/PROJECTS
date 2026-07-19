using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Models.Interfaces;

public interface IActivityService
{
    Task LogAsync(int userId, string userName, string userRole,
                  string action, string entityType, string description);
    Task<IEnumerable<ActivityLog>> GetRecentAsync(int count = 60);
}
