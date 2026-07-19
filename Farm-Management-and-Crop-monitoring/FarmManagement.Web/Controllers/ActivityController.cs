using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ActivityController : Controller
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _activityService.GetRecentAsync(100);
        return View(logs);
    }
}
