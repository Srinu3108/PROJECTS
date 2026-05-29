using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

// Builder Pattern — constructs a DashboardViewModel step by step.
// Each With___() method populates one section; Build() returns the finished object.
public class DashboardBuilder
{
    private readonly FarmDbContext      _db;
    private readonly DashboardViewModel _vm = new();

    public DashboardBuilder(FarmDbContext db) => _db = db;

    public async Task<DashboardBuilder> WithFieldCountAsync()
    {
        _vm.TotalFields = await _db.Fields.CountAsync();
        return this;
    }

    public async Task<DashboardBuilder> WithCropCountAsync()
    {
        _vm.TotalCrops = await _db.Crops.CountAsync();
        return this;
    }

    public async Task<DashboardBuilder> WithActivePestCountAsync()
    {
        _vm.ActivePestIncidents = await _db.PestIncidents
            .CountAsync(p => p.Status == IncidentStatus.Active);
        return this;
    }

    public async Task<DashboardBuilder> WithLowStockCountAsync()
    {
        _vm.LowStockResources = await _db.Resources
            .CountAsync(r => r.Quantity <= 10);
        return this;
    }

    public async Task<DashboardBuilder> WithUpcomingHarvestsAsync()
    {
        _vm.UpcomingHarvests = await _db.PlantingSchedules
            .CountAsync(ps => ps.ScheduledDate >= DateTime.Today
                           && ps.ScheduledDate <= DateTime.Today.AddDays(30)
                           && ps.Status == "Scheduled");
        return this;
    }

    public async Task<DashboardBuilder> WithTotalYieldAsync()
    {
        _vm.TotalYieldThisSeason = await _db.Harvests
            .Where(h => h.HarvestedDate.Year == DateTime.Today.Year)
            .SumAsync(h => (decimal?)h.ActualYieldKg) ?? 0;
        return this;
    }

    public async Task<DashboardBuilder> WithRecentSchedulesAsync()
    {
        _vm.RecentSchedules = await _db.PlantingSchedules
            .Include(ps => ps.Crop)
            .Where(ps => ps.Status == "Scheduled")
            .OrderBy(ps => ps.ScheduledDate)
            .Take(5)
            .ToListAsync();
        return this;
    }

    public async Task<DashboardBuilder> WithRecentPestAlertsAsync()
    {
        _vm.RecentPestAlerts = await _db.PestIncidents
            .Include(p => p.Crop)
            .Where(p => p.Status == IncidentStatus.Active)
            .OrderByDescending(p => p.ReportedDate)
            .Take(5)
            .ToListAsync();
        return this;
    }

    // Build — returns the fully assembled DashboardViewModel
    public DashboardViewModel Build() => _vm;
}
