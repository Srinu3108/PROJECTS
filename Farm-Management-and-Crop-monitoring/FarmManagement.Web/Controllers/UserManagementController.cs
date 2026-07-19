using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UserManagementController : Controller
{
    private readonly IUserManagementService _userService;
    private readonly IAccountService        _accountService;
    private readonly IActivityService       _activityService;

    public UserManagementController(IUserManagementService userService,
                                    IAccountService        accountService,
                                    IActivityService       activityService)
    {
        _userService      = userService;
        _accountService   = accountService;
        _activityService  = activityService;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name)  ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role)  ?? "Unknown";

    public async Task<IActionResult> Index()
    {
        var users      = await _userService.GetAllUsersAsync();
        var activities = await _activityService.GetRecentAsync(60);
        ViewBag.RecentActivities = activities;
        return View(users);
    }

    // ── Register new user (Admin only) ──────────────────────────────
    public IActionResult RegisterUser() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterUser(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (await _accountService.EmailExistsAsync(vm.Email))
        {
            ModelState.AddModelError("Email", "This email address is already registered.");
            return View(vm);
        }

        var success = await _accountService.RegisterAsync(vm);
        if (!success)
        {
            TempData["Error"] = "Registration failed. Please try again.";
            return View(vm);
        }

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Registered", "User", $"Created account for {vm.FullName} ({vm.Email}) as {vm.Role}");

        TempData["Success"] = $"Account for '{vm.FullName}' created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Update role ─────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(int userId, UserRole role)
    {
        if (userId == CurrentUserId)
        {
            TempData["Error"] = "You cannot change your own role.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        if (user.Role == UserRole.Admin)
        {
            TempData["Error"] = "Cannot change the role of an administrator.";
            return RedirectToAction(nameof(Index));
        }

        var oldRole = user.Role;
        await _userService.UpdateRoleAsync(userId, role);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "RoleChanged", "User", $"Changed {user.FullName}'s role from {oldRole} to {role}");

        TempData["Success"] = $"Role for '{user.FullName}' updated to {role}.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete user ─────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int userId)
    {
        if (userId == CurrentUserId)
        {
            TempData["Error"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        if (user.Role == UserRole.Admin)
        {
            TempData["Error"] = "Cannot remove an administrator account.";
            return RedirectToAction(nameof(Index));
        }

        await _userService.DeleteUserAsync(userId);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Deleted", "User", $"Removed account: {user.FullName} ({user.Email})");

        TempData["Success"] = $"User '{user.FullName}' has been removed.";
        return RedirectToAction(nameof(Index));
    }

    // ── Suspend / Activate user ─────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBlock(int userId)
    {
        if (userId == CurrentUserId)
        {
            TempData["Error"] = "You cannot suspend your own account.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        if (user.Role == UserRole.Admin)
        {
            TempData["Error"] = "Cannot suspend an administrator account.";
            return RedirectToAction(nameof(Index));
        }

        await _userService.ToggleBlockAsync(userId);

        var action = user.IsBlocked ? "Activated" : "Suspended";
        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            action, "User", $"{action} account: {user.FullName} ({user.Email})");

        TempData["Success"] = $"Account for '{user.FullName}' has been {action.ToLower()}.";
        return RedirectToAction(nameof(Index));
    }
}
