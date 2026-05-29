using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Models.ViewModels;

public class ResourceReportViewModel
{
    public int TotalResources { get; set; }
    public int LowStockCount { get; set; }
    public int TotalAllocations { get; set; }
    public IEnumerable<Resource> Resources { get; set; } = new List<Resource>();
    public List<TypeCount> TypeBreakdown { get; set; } = new();
}
