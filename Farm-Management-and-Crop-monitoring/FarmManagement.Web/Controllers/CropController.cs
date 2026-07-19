using FarmManagement.Web.Events;
using FarmManagement.Web.Factories;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

// Patterns used:
//   Factory  — ICropFactory maps entity ↔ ViewModel (no manual mapping in controller)
//   Observer — IEventDispatcher replaces direct IActivityService calls
//   Facade   — IFarmFacade handles Delete (service + event in one call)
[Authorize(Roles = "Admin,Farmer,FieldSupervisor,Storekeeper,Agronomist")]
public class CropController : Controller
{
    private readonly ICropService     _cropService;
    private readonly ICropFactory     _cropFactory;
    private readonly IEventDispatcher _dispatcher;
    private readonly IFarmFacade      _facade;

    public CropController(ICropService cropService, ICropFactory cropFactory,
                          IEventDispatcher dispatcher, IFarmFacade facade)
    {
        _cropService = cropService;
        _cropFactory = cropFactory;
        _dispatcher  = dispatcher;
        _facade      = facade;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index(string? search)
    {
        var crops = await _cropService.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            crops = crops.Where(c => c.CropName.Contains(search, StringComparison.OrdinalIgnoreCase)
                                  || c.CropType.Contains(search, StringComparison.OrdinalIgnoreCase)
                                  || c.Status.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        ViewBag.Search = search;
        return View(crops);
    }

    public async Task<IActionResult> Details(int id)
    {
        var crop = await _cropService.GetByIdAsync(id);
        if (crop == null) return NotFound();
        return View(crop);
    }

    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Create()
    {
        var vm = await _cropService.PrepareViewModelAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Create(CropViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var prepared = await _cropService.PrepareViewModelAsync(vm);
            return View(prepared);
        }

        await _cropService.CreateAsync(vm);

        // Observer Pattern — dispatch event; CropCreatedHandler writes the activity log
        await _dispatcher.DispatchAsync(new CropCreatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            vm.CropName, vm.CropType, vm.Season.ToString()));

        TempData["Success"] = $"Crop '{vm.CropName}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Edit(int id)
    {
        var crop = await _cropService.GetByIdAsync(id);
        if (crop == null) return NotFound();

        // Factory Pattern — single line replaces 8-line manual ViewModel construction
        var vm = _cropFactory.ToViewModel(crop);

        var prepared = await _cropService.PrepareViewModelAsync(vm);
        return View(prepared);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Edit(int id, CropViewModel vm)
    {
        if (id != vm.CropId) return BadRequest();

        if (!ModelState.IsValid)
        {
            var prepared = await _cropService.PrepareViewModelAsync(vm);
            return View(prepared);
        }

        await _cropService.UpdateAsync(vm);

        // Observer Pattern
        await _dispatcher.DispatchAsync(new CropUpdatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            vm.CropName, vm.Status ?? "Growing"));

        TempData["Success"] = $"Crop '{vm.CropName}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var crop = await _cropService.GetByIdAsync(id);
        if (crop == null) return NotFound();

        // Facade Pattern — single call coordinates CropService + event dispatch
        await _facade.DeleteCropAsync(
            id, crop.CropName,
            CurrentUserId, CurrentUserName, CurrentUserRole);

        TempData["Success"] = $"Crop '{crop.CropName}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}
