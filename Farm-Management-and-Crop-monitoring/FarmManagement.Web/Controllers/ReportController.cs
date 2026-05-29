using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers;

[Authorize(Roles = "Admin,Farmer,FieldSupervisor,Storekeeper,Agronomist")]
public class ReportController : Controller
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = await _reportService.GetReportDashboardAsync();
        return View(vm);
    }

    public async Task<IActionResult> YieldAnalytics()
    {
        var vm = await _reportService.GetYieldAnalyticsAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,Agronomist")]
    public async Task<IActionResult> GenerateYieldReport()
    {
        await _reportService.GenerateYieldReportAsync();
        TempData["Success"] = "Yield report generated successfully.";
        return RedirectToAction(nameof(YieldReports));
    }

    public async Task<IActionResult> YieldReports()
    {
        var reports = await _reportService.GetYieldReportsAsync();
        return View(reports);
    }

    public async Task<IActionResult> PestSummary()
    {
        var vm = await _reportService.GetPestSummaryAsync();
        return View(vm);
    }

    public async Task<IActionResult> ResourceReport()
    {
        var vm = await _reportService.GetResourceReportAsync();
        return View(vm);
    }

    public async Task<IActionResult> FarmAnalytics()
    {
        var vm = await _reportService.GetFarmAnalyticsAsync();
        return View(vm);
    }
}
