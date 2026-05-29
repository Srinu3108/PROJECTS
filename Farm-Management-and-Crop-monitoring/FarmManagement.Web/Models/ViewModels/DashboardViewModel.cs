using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Models.ViewModels;

public class DashboardViewModel
{
    public int TotalFields { get; set; }
    public int TotalCrops { get; set; }
    public int ActivePestIncidents { get; set; }
    public int LowStockResources { get; set; }
    public int UpcomingHarvests { get; set; }
    public decimal TotalYieldThisSeason { get; set; }

    public IEnumerable<PlantingSchedule> RecentSchedules { get; set; } = new List<PlantingSchedule>();
    public IEnumerable<PestIncident> RecentPestAlerts { get; set; } = new List<PestIncident>();
}