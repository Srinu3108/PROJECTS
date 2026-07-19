using FarmManagement.Web.Events;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

// Patterns used:
//   Observer — IEventDispatcher replaces direct IActivityService calls
//   Facade   — IFarmFacade handles RecordHarvest & Delete (multi-service ops)
[Authorize(Roles = "Admin,Farmer,FieldSupervisor,Storekeeper,Agronomist")]
public class ScheduleController : Controller
{
    private readonly IScheduleService  _scheduleService;
    private readonly IEventDispatcher  _dispatcher;
    private readonly IFarmFacade       _facade;
    private readonly IResourceService  _resourceService;

    public ScheduleController(IScheduleService scheduleService,
                               IEventDispatcher dispatcher,
                               IFarmFacade facade,
                               IResourceService resourceService)
    {
        _scheduleService = scheduleService;
        _dispatcher      = dispatcher;
        _facade          = facade;
        _resourceService = resourceService;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index(string? search)
    {
        var schedules = await _scheduleService.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            schedules = schedules.Where(s => (s.Crop?.CropName ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)
                                          || (s.Field?.FieldName ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)
                                          || s.Status.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        ViewBag.Search = search;
        return View(schedules);
    }

    public async Task<IActionResult> HarvestList()
    {
        var upcoming = await _scheduleService.GetUpcomingAsync();
        return View(upcoming);
    }

    public async Task<IActionResult> Details(int id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);
        if (schedule == null) return NotFound();
        return View(schedule);
    }

    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Create()
    {
        var vm = await _scheduleService.PrepareViewModelAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Create(ScheduleViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var prepared = await _scheduleService.PrepareViewModelAsync(vm);
            return View(prepared);
        }

        await _scheduleService.CreateAsync(vm);

        // Observer Pattern
        await _dispatcher.DispatchAsync(new ScheduleCreatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            vm.ScheduledDate, vm.ExpectedYieldKg));

        TempData["Success"] = "Harvest scheduled successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Edit(int id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);
        if (schedule == null) return NotFound();

        var vm = await _scheduleService.PrepareViewModelAsync(new ScheduleViewModel
        {
            ScheduleId = schedule.ScheduleId,
            CropId = schedule.CropId,
            FieldId = schedule.FieldId,
            ScheduledDate = schedule.ScheduledDate,
            ExpectedYieldKg = schedule.ExpectedYieldKg,
            Notes = schedule.Notes,
            Status = schedule.Status,
            ResourceUsages = schedule.ResourceUsages?.Select(ru => new FarmManagement.Web.Models.ViewModels.ResourceUsageItem
            {
                ResourceId   = ru.ResourceId,
                QuantityUsed = ru.QuantityUsed,
                Notes        = ru.Notes
            }).ToList() ?? new()
        });
        ViewBag.ExistingResources = schedule.ResourceUsages?.Select(ru => new
        {
            ru.ResourceId,
            Name = ru.Resource?.Name ?? "Unknown",
            Type = ru.Resource?.Type.ToString() ?? "",
            Unit = ru.Resource?.Unit ?? "",
            ru.QuantityUsed,
            Notes = ru.Notes ?? ""
        }).ToList();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Edit(ScheduleViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var prepared = await _scheduleService.PrepareViewModelAsync(vm);
            ViewBag.ExistingResources = new List<object>();
            return View(prepared);
        }

        await _scheduleService.UpdateAsync(vm);
        TempData["Success"] = "Schedule updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Farmer,FieldSupervisor,Storekeeper")]
    public async Task<IActionResult> RecordHarvest(int id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);
        if (schedule == null) return NotFound();

        if (schedule.Status == "Completed")
        {
            TempData["Error"] = "This harvest has already been recorded.";
            return RedirectToAction(nameof(HarvestList));
        }

        return View(schedule);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor,Storekeeper")]
    public async Task<IActionResult> RecordHarvest(int id, decimal actualYield, string? notes)
    {
        if (actualYield <= 0)
        {
            TempData["Error"] = "Actual yield must be greater than 0.";
            return RedirectToAction(nameof(RecordHarvest), new { id });
        }

        // Facade Pattern — single call coordinates ScheduleService + event dispatch
        await _facade.RecordHarvestAsync(
            id, actualYield, notes,
            CurrentUserId, CurrentUserName, CurrentUserRole);

        TempData["Success"] = "Harvest recorded successfully.";
        return RedirectToAction(nameof(HarvestList));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Facade Pattern — single call coordinates ScheduleService + event dispatch
        await _facade.DeleteScheduleAsync(
            id, CurrentUserId, CurrentUserName, CurrentUserRole);

        TempData["Success"] = "Schedule deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> GetResourcesJson()
    {
        var resources = await _resourceService.GetAllAsync();
        var list = resources
            .Where(r => r.Type != FarmManagement.Web.Models.Enums.ResourceType.Pesticide)
            .Select(r => new
            {
                r.ResourceId,
                r.Name,
                Type = r.Type.ToString(),
                r.Quantity,
                r.Unit,
                LastUpdated = r.LastUpdated.ToString("dd MMM yyyy")
            });
        return Json(list);
    }
}
