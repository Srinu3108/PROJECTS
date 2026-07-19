using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Models.ViewModels;

public class ReportDashboardViewModel
{
    // ── Summary Stats ──
    public int TotalFields { get; set; }
    public decimal TotalFieldArea { get; set; }
    public int TotalCrops { get; set; }
    public int TotalResources { get; set; }
    public int LowStockCount { get; set; }
    public int TotalSchedules { get; set; }
    public int ScheduledCount { get; set; }
    public int CompletedCount { get; set; }
    public int TotalHarvests { get; set; }
    public decimal TotalYieldKg { get; set; }
    public decimal AverageYieldKg { get; set; }
    public int TotalPestIncidents { get; set; }
    public int ActivePests { get; set; }
    public int MonitoringPests { get; set; }
    public int ResolvedPests { get; set; }

    // ── History Tables ──
    public IEnumerable<Field> FieldHistory { get; set; } = new List<Field>();
    public IEnumerable<Crop> CropHistory { get; set; } = new List<Crop>();
    public IEnumerable<PestIncident> PestHistory { get; set; } = new List<PestIncident>();
    public IEnumerable<Resource> ResourceHistory { get; set; } = new List<Resource>();
    public IEnumerable<Harvest> HarvestHistory { get; set; } = new List<Harvest>();
    public IEnumerable<PlantingSchedule> ScheduleHistory { get; set; } = new List<PlantingSchedule>();

    // ── Breakdowns ──
    public List<CropPestCount> PestsByCrop { get; set; } = new();
    public List<TypeCount> ResourcesByType { get; set; } = new();
    public List<StatusCount> CropsByStatus { get; set; } = new();
}
