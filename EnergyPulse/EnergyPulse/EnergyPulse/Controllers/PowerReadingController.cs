using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EnergyPulse.Models;
using EnergyPulse.Services;

namespace EnergyPulse.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PowerReadingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AlertService _alertService;

        public PowerReadingController(AppDbContext context, AlertService alertService)
        {
            _context = context;
            _alertService = alertService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReading(PowerReading reading)
        {
            _context.PowerReadings.Add(reading);
            await _context.SaveChangesAsync();

            // 🔥 SMART LOGIC TRIGGER
            await _alertService.CheckAndCreateAlert(reading);

            return Ok("Reading added and alert checked");
        }
    }
}
