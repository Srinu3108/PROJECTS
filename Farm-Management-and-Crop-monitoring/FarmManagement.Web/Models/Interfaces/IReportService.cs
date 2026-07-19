using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Interfaces;

public interface IReportService
{
    Task<DashboardViewModel> GetDashboardDataAsync();
    Task<YieldAnalyticsViewModel> GetYieldAnalyticsAsync();
    Task GenerateYieldReportAsync();
    Task<IEnumerable<YieldReport>> GetYieldReportsAsync();
    Task<PestSummaryViewModel> GetPestSummaryAsync();
    Task<ResourceReportViewModel> GetResourceReportAsync();
    Task<FarmAnalyticsViewModel> GetFarmAnalyticsAsync();
    Task<ReportDashboardViewModel> GetReportDashboardAsync();
}