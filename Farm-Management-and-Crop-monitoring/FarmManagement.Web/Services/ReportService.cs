using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class ReportService : IReportService
{
    private readonly FarmDbContext _db;
    public ReportService(FarmDbContext db) => _db = db;

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var builder = new DashboardBuilder(_db);

        await builder.WithFieldCountAsync();
        await builder.WithCropCountAsync();
        await builder.WithActivePestCountAsync();
        await builder.WithLowStockCountAsync();
        await builder.WithUpcomingHarvestsAsync();
        await builder.WithTotalYieldAsync();
        await builder.WithRecentSchedulesAsync();
        await builder.WithRecentPestAlertsAsync();

        return builder.Build();
    }

    public async Task<YieldAnalyticsViewModel> GetYieldAnalyticsAsync()
    {
        var records = await _db.Harvests.AsNoTracking()
                               .Include(h => h.PlantingSchedule)
                                   .ThenInclude(ps => ps.Crop)
                               .Include(h => h.PlantingSchedule)
                                   .ThenInclude(ps => ps.Field)
                               .OrderByDescending(h => h.HarvestedDate)
                               .ToListAsync();

        return new YieldAnalyticsViewModel
        {
            Records      = records,
            CropNames    = records.Select(r => r.PlantingSchedule.Crop.CropName).ToList(),
            YieldValues  = records.Select(r => r.ActualYieldKg).ToList(),
            TotalYield   = records.Sum(r => r.ActualYieldKg),
            AverageYield = records.Count > 0 ? records.Average(r => r.ActualYieldKg) : 0
        };
    }

    public async Task GenerateYieldReportAsync()
    {
        var harvests = await _db.Harvests.AsNoTracking()
            .Include(h => h.PlantingSchedule).ThenInclude(ps => ps.Crop)
            .Include(h => h.PlantingSchedule).ThenInclude(ps => ps.Field)
            .Where(h => h.HarvestedDate.Year == DateTime.Today.Year)
            .ToListAsync();

        var grouped = harvests.GroupBy(h => new
        {
            h.PlantingSchedule.CropId,
            h.PlantingSchedule.Crop.Season
        });

        foreach (var group in grouped)
        {
            var totalYield = group.Sum(h => h.ActualYieldKg);
            var totalArea = group.Select(h => h.PlantingSchedule.Field)
                                 .DistinctBy(f => f.FieldId)
                                 .Sum(f => f.AreaHectares);
            var avgPerAcre = totalArea > 0 ? totalYield / totalArea : 0;

            var existing = await _db.YieldReports.FirstOrDefaultAsync(y =>
                y.CropId == group.Key.CropId &&
                y.Season == group.Key.Season &&
                y.Year == DateTime.Today.Year);

            if (existing != null)
            {
                existing.TotalYieldKg = totalYield;
                existing.AverageYieldPerAcre = avgPerAcre;
                existing.GeneratedAt = DateTime.Now;
                existing.Remarks = $"Auto-generated from {group.Count()} harvest records";
            }
            else
            {
                _db.YieldReports.Add(new YieldReport
                {
                    CropId = group.Key.CropId,
                    TotalYieldKg = totalYield,
                    AverageYieldPerAcre = avgPerAcre,
                    Season = group.Key.Season,
                    Year = DateTime.Today.Year,
                    Remarks = $"Auto-generated from {group.Count()} harvest records"
                });
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<YieldReport>> GetYieldReportsAsync() =>
        await _db.YieldReports.AsNoTracking()
            .Include(y => y.Crop)
            .OrderByDescending(y => y.GeneratedAt)
            .ToListAsync();

    public async Task<PestSummaryViewModel> GetPestSummaryAsync()
    {
        var incidents = await _db.PestIncidents.AsNoTracking()
            .Include(p => p.Crop)
            .ToListAsync();

        var cropBreakdown = incidents
            .GroupBy(p => p.Crop?.CropName ?? "Unknown")
            .Select(g => new CropPestCount
            {
                CropName   = g.Key,
                Active     = g.Count(p => p.Status == IncidentStatus.Active),
                Monitoring = g.Count(p => p.Status == IncidentStatus.Monitoring),
                Resolved   = g.Count(p => p.Status == IncidentStatus.Resolved)
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        return new PestSummaryViewModel
        {
            TotalIncidents    = incidents.Count,
            ActiveCount       = incidents.Count(p => p.Status == IncidentStatus.Active),
            MonitoringCount   = incidents.Count(p => p.Status == IncidentStatus.Monitoring),
            ResolvedCount     = incidents.Count(p => p.Status == IncidentStatus.Resolved),
            Incidents         = incidents.OrderByDescending(p => p.ReportedDate),
            CropWiseBreakdown = cropBreakdown
        };
    }

    public async Task<ResourceReportViewModel> GetResourceReportAsync()
    {
        var resources = await _db.Resources.AsNoTracking()
            .Include(r => r.ResourceUsages)
            .Where(r => r.Type != Models.Enums.ResourceType.Pesticide)
            .OrderBy(r => r.Name)
            .ToListAsync();

        var typeBreakdown = resources
            .GroupBy(r => r.Type)
            .Select(g => new TypeCount
            {
                Type     = g.Key.ToString(),
                Count    = g.Count(),
                TotalQty = g.Sum(r => r.Quantity)
            })
            .OrderByDescending(t => t.Count)
            .ToList();

        return new ResourceReportViewModel
        {
            TotalResources   = resources.Count,
            LowStockCount    = resources.Count(r => r.Quantity <= 10),
            TotalAllocations = resources.Sum(r => r.ResourceUsages.Count),
            Resources        = resources,
            TypeBreakdown    = typeBreakdown
        };
    }

    public async Task<FarmAnalyticsViewModel> GetFarmAnalyticsAsync()
    {
        var pestIncidents = await _db.PestIncidents.AsNoTracking()
            .Include(p => p.Crop)
            .ToListAsync();

        var pestsByCrop = pestIncidents
            .GroupBy(p => p.Crop?.CropName ?? "Unknown")
            .Select(g => new CropPestCount
            {
                CropName   = g.Key,
                Active     = g.Count(p => p.Status == IncidentStatus.Active),
                Monitoring = g.Count(p => p.Status == IncidentStatus.Monitoring),
                Resolved   = g.Count(p => p.Status == IncidentStatus.Resolved)
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        return new FarmAnalyticsViewModel
        {
            TotalFields = await _db.Fields.CountAsync(),
            TotalCrops = await _db.Crops.CountAsync(),
            TotalResources = await _db.Resources.CountAsync(r => r.Type != Models.Enums.ResourceType.Pesticide),
            TotalHarvests = await _db.Harvests.CountAsync(),
            TotalPestIncidents = pestIncidents.Count,
            ActivePests = pestIncidents.Count(p => p.Status == IncidentStatus.Active),
            LowStockItems = await _db.Resources.CountAsync(r => r.Quantity <= 10 && r.Type != Models.Enums.ResourceType.Pesticide),
            TotalYieldKg = await _db.Harvests.SumAsync(h => (decimal?)h.ActualYieldKg) ?? 0,
            TotalFieldArea = await _db.Fields.SumAsync(f => (decimal?)f.AreaHectares) ?? 0,
            TotalSchedules = await _db.PlantingSchedules.CountAsync(),
            ScheduledCount = await _db.PlantingSchedules.CountAsync(s => s.Status == "Scheduled"),
            CompletedCount = await _db.PlantingSchedules.CountAsync(s => s.Status == "Completed"),
            CropsByStatus = await _db.Crops.AsNoTracking()
                .GroupBy(c => c.Status)
                .Select(g => new StatusCount { Status = g.Key, Count = g.Count() })
                .ToListAsync(),
            ResourcesByType = await _db.Resources.AsNoTracking()
                .Where(r => r.Type != Models.Enums.ResourceType.Pesticide)
                .GroupBy(r => r.Type)
                .Select(g => new TypeCount { Type = g.Key.ToString(), Count = g.Count(), TotalQty = g.Sum(r => r.Quantity) })
                .ToListAsync(),
            PestsByCrop = pestsByCrop
        };
    }

    public async Task<ReportDashboardViewModel> GetReportDashboardAsync()
    {
        var fields = await _db.Fields.AsNoTracking()
            .Include(f => f.Crops)
            .OrderBy(f => f.FieldName)
            .ToListAsync();

        var crops = await _db.Crops.AsNoTracking()
            .Include(c => c.Field)
            .OrderBy(c => c.CropName)
            .ToListAsync();

        var pestIncidents = await _db.PestIncidents.AsNoTracking()
            .Include(p => p.Crop)
            .OrderByDescending(p => p.ReportedDate)
            .ToListAsync();

        var resources = await _db.Resources.AsNoTracking()
            .Include(r => r.ResourceUsages)
            .Where(r => r.Type != Models.Enums.ResourceType.Pesticide)
            .OrderBy(r => r.Name)
            .ToListAsync();

        var harvests = await _db.Harvests.AsNoTracking()
            .Include(h => h.PlantingSchedule).ThenInclude(ps => ps.Crop)
            .Include(h => h.PlantingSchedule).ThenInclude(ps => ps.Field)
            .OrderByDescending(h => h.HarvestedDate)
            .ToListAsync();

        var schedules = await _db.PlantingSchedules.AsNoTracking()
            .Include(ps => ps.Crop)
            .Include(ps => ps.Field)
            .OrderByDescending(ps => ps.ScheduledDate)
            .ToListAsync();

        var pestsByCrop = pestIncidents
            .GroupBy(p => p.Crop?.CropName ?? "Unknown")
            .Select(g => new CropPestCount
            {
                CropName   = g.Key,
                Active     = g.Count(p => p.Status == IncidentStatus.Active),
                Monitoring = g.Count(p => p.Status == IncidentStatus.Monitoring),
                Resolved   = g.Count(p => p.Status == IncidentStatus.Resolved)
            }).OrderByDescending(c => c.Total).ToList();

        var resourcesByType = resources
            .GroupBy(r => r.Type)
            .Select(g => new TypeCount { Type = g.Key.ToString(), Count = g.Count(), TotalQty = g.Sum(r => r.Quantity) })
            .OrderByDescending(t => t.Count).ToList();

        var cropsByStatus = await _db.Crops.AsNoTracking()
            .GroupBy(c => c.Status)
            .Select(g => new StatusCount { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return new ReportDashboardViewModel
        {
            TotalFields        = fields.Count,
            TotalFieldArea     = fields.Sum(f => f.AreaHectares),
            TotalCrops         = crops.Count,
            TotalResources     = resources.Count,
            LowStockCount      = resources.Count(r => r.Quantity <= 10),
            TotalSchedules     = schedules.Count,
            ScheduledCount     = schedules.Count(s => s.Status == "Scheduled"),
            CompletedCount     = schedules.Count(s => s.Status == "Completed"),
            TotalHarvests      = harvests.Count,
            TotalYieldKg       = harvests.Sum(h => h.ActualYieldKg),
            AverageYieldKg     = harvests.Count > 0 ? harvests.Average(h => h.ActualYieldKg) : 0,
            TotalPestIncidents = pestIncidents.Count,
            ActivePests        = pestIncidents.Count(p => p.Status == IncidentStatus.Active),
            MonitoringPests    = pestIncidents.Count(p => p.Status == IncidentStatus.Monitoring),
            ResolvedPests      = pestIncidents.Count(p => p.Status == IncidentStatus.Resolved),
            FieldHistory       = fields,
            CropHistory        = crops,
            PestHistory        = pestIncidents,
            ResourceHistory    = resources,
            HarvestHistory     = harvests,
            ScheduleHistory    = schedules,
            PestsByCrop        = pestsByCrop,
            ResourcesByType    = resourcesByType,
            CropsByStatus      = cropsByStatus
        };
    }
}
