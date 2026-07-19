using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EnergyPulse.Services;

namespace EnergyPulse.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _service;

        public DashboardController(DashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var data = await _service.GetDashboardData();
            return Ok(data);
        }
    }
}
