using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Models.ViewModels;

public class PestSummaryViewModel
{
    public int TotalIncidents { get; set; }
    public int ActiveCount { get; set; }
    public int MonitoringCount { get; set; }
    public int ResolvedCount { get; set; }
    public IEnumerable<PestIncident> Incidents { get; set; } = new List<PestIncident>();
    public List<CropPestCount> CropWiseBreakdown { get; set; } = new();
}

public class CropPestCount
{
    public string CropName { get; set; } = string.Empty;
    public int Active { get; set; }
    public int Monitoring { get; set; }
    public int Resolved { get; set; }
    public int Total => Active + Monitoring + Resolved;
}
