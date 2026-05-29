namespace FarmManagement.Web.Models.ViewModels;

public class FarmAnalyticsViewModel
{
    public int TotalFields { get; set; }
    public int TotalCrops { get; set; }
    public int TotalResources { get; set; }
    public int TotalHarvests { get; set; }
    public int TotalPestIncidents { get; set; }
    public int ActivePests { get; set; }
    public int LowStockItems { get; set; }
    public decimal TotalYieldKg { get; set; }
    public decimal TotalFieldArea { get; set; }
    public int TotalSchedules { get; set; }
    public int ScheduledCount { get; set; }
    public int CompletedCount { get; set; }
    public List<StatusCount> CropsByStatus { get; set; } = new();
    public List<TypeCount> ResourcesByType { get; set; } = new();
    public List<CropPestCount> PestsByCrop { get; set; } = new();
}

public class StatusCount
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TypeCount
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalQty { get; set; }
}
