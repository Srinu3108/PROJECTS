using FarmManagement.Web.Events;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using FarmManagement.Web.States;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

// Patterns used:
//   State    — PestStateMachine enforces valid status transitions (Active→Monitoring→Resolved)
//   Observer — IEventDispatcher replaces direct IActivityService calls
//   Facade   — IFarmFacade handles Delete (service + event in one call)
[Authorize(Roles = "Admin,Farmer,FieldSupervisor,Storekeeper,Agronomist")]
public class PestController : Controller
{
    private readonly IPestService      _pestService;
    private readonly ICropService      _cropService;
    private readonly IResourceService  _resourceService;
    private readonly IEventDispatcher  _dispatcher;
    private readonly IFarmFacade       _facade;

    public PestController(IPestService pestService, ICropService cropService,
                          IResourceService resourceService,
                          IEventDispatcher dispatcher, IFarmFacade facade)
    {
        _pestService     = pestService;
        _cropService     = cropService;
        _resourceService = resourceService;
        _dispatcher      = dispatcher;
        _facade          = facade;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index(string? search)
    {
        var incidents = await _pestService.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            incidents = incidents.Where(p => p.PestName.Contains(search, StringComparison.OrdinalIgnoreCase)
                                          || (p.Crop?.CropName ?? "").Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        ViewBag.Search = search;
        return View(incidents);
    }

    public async Task<IActionResult> Details(int id)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();
        return View(incident);
    }

    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Crops = await _cropService.GetAllAsync();
        return View(new PestIncident());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Create(PestIncident pest)
    {
        ModelState.Remove("Crop");

        if (!ModelState.IsValid)
        {
            ViewBag.Crops = await _cropService.GetAllAsync();
            return View(pest);
        }

        pest.ReportedDate = DateTime.Now;
        pest.Status       = IncidentStatus.Active;

        await _pestService.CreateAsync(pest);

        // Observer Pattern
        await _dispatcher.DispatchAsync(new PestReportedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole, pest.PestName));

        TempData["Success"] = $"Pest incident '{pest.PestName}' reported successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Farmer,FieldSupervisor,Agronomist")]
    public async Task<IActionResult> Edit(int id)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        ViewBag.Crops = await _cropService.GetAllAsync();
        return View(incident);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor,Agronomist")]
    public async Task<IActionResult> Edit(int id, PestIncident pest)
    {
        if (id != pest.PestIncidentId) return BadRequest();
        ModelState.Remove("Crop");

        if (!ModelState.IsValid)
        {
            ViewBag.Crops = await _cropService.GetAllAsync();
            return View(pest);
        }

        await _pestService.UpdateAsync(pest);

        await _dispatcher.DispatchAsync(new PestStatusUpdatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            pest.PestName, pest.Status.ToString()));

        TempData["Success"] = $"Pest incident '{pest.PestName}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor,Agronomist")]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        try
        {
            // State Pattern — PestService uses PestStateMachine to validate the transition
            await _pestService.UpdateStatusAsync(id, status);

            // Observer Pattern
            await _dispatcher.DispatchAsync(new PestStatusUpdatedEvent(
                CurrentUserId, CurrentUserName, CurrentUserRole,
                incident.PestName, status));
            TempData["Success"] = $"Status updated to '{status}' for '{incident.PestName}'.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        // Facade Pattern — single call coordinates PestService + event dispatch
        await _facade.DeletePestAsync(
            id, incident.PestName,
            CurrentUserId, CurrentUserName, CurrentUserRole);

        TempData["Success"] = $"Incident '{incident.PestName}' deleted.";
        return RedirectToAction(nameof(Index));
    }

    // ── Pest Resource Management (Pesticide type only) ──

    public async Task<IActionResult> AddResource(int id)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        ViewBag.PestIncidentId = id;
        ViewBag.PestName = incident.PestName;
        var vm = new InventoryViewModel { Type = ResourceType.Pesticide };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddResource(int id, InventoryViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PestIncidentId = id;
            var incident = await _pestService.GetByIdAsync(id);
            ViewBag.PestName = incident?.PestName;
            return View(vm);
        }

        vm.Type = ResourceType.Pesticide;
        await _resourceService.CreateAsync(vm, id);

        await _dispatcher.DispatchAsync(new ResourceCreatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            vm.Name, vm.Quantity, vm.Unit));

        TempData["Success"] = $"Pesticide resource '{vm.Name}' added to incident.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> EditResource(int id, int pestId)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        ViewBag.PestIncidentId = pestId;
        var incident = await _pestService.GetByIdAsync(pestId);
        ViewBag.PestName = incident?.PestName;

        var vm = new InventoryViewModel
        {
            ResourceId = resource.ResourceId,
            Name = resource.Name,
            Type = resource.Type,
            Quantity = resource.Quantity,
            Unit = resource.Unit
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditResource(int id, int pestId, InventoryViewModel vm)
    {
        if (id != vm.ResourceId) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.PestIncidentId = pestId;
            var incident = await _pestService.GetByIdAsync(pestId);
            ViewBag.PestName = incident?.PestName;
            return View(vm);
        }

        vm.Type = ResourceType.Pesticide;
        await _resourceService.UpdateAsync(vm);

        await _dispatcher.DispatchAsync(new ResourceUpdatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            vm.Name, vm.Quantity, vm.Unit));

        TempData["Success"] = $"Pesticide resource '{vm.Name}' updated.";
        return RedirectToAction(nameof(Details), new { id = pestId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteResource(int id, int pestId)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        await _facade.DeleteResourceAsync(
            id, resource.Name,
            CurrentUserId, CurrentUserName, CurrentUserRole);

        TempData["Success"] = $"Pesticide resource '{resource.Name}' removed from incident.";
        return RedirectToAction(nameof(Details), new { id = pestId });
    }
}
