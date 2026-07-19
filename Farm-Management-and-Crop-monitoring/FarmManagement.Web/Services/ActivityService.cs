using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class ActivityService : IActivityService
{
    private readonly FarmDbContext _db;
    public ActivityService(FarmDbContext db) => _db = db;

    public async Task LogAsync(int userId, string userName, string userRole,
                               string action, string entityType, string description)
    {
        _db.ActivityLogs.Add(new ActivityLog
        {
            UserId      = userId,
            UserName    = userName,
            UserRole    = userRole,
            Action      = action,
            EntityType  = entityType,
            Description = description,
            Timestamp   = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<ActivityLog>> GetRecentAsync(int count = 60)
        => await _db.ActivityLogs.AsNoTracking()
                    .OrderByDescending(a => a.Timestamp)
                    .Take(count)
                    .ToListAsync();
}
