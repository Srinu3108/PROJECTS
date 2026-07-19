using Microsoft.AspNetCore.Mvc;
using EnergyPulse.DTOs;
using EnergyPulse.Models;
using EnergyPulse.Services;
using Microsoft.EntityFrameworkCore;

namespace EnergyPulse.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AlertService _alertService;
        private readonly ILogger<AlertsController> _logger;

        public AlertsController(AppDbContext context, AlertService alertService, ILogger<AlertsController> logger)
        {
            _context = context;
            _alertService = alertService;
            _logger = logger;
        }

        /// <summary>
        /// Get all alerts with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAlerts(
            [FromQuery] int? siteId = null,
            [FromQuery] string severity = null,
            [FromQuery] string status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Alerts
                    .Include(a => a.Device)
                    .AsQueryable();

                if (siteId.HasValue)
                    query = query.Where(a => a.Device.SiteId == siteId);

                if (!string.IsNullOrEmpty(severity))
                    query = query.Where(a => a.Severity == severity);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(a => a.Status == status);

                var totalCount = await query.CountAsync();

                var alerts = await query
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new AlertDto
                    {
                        Id = a.Id,
                        DeviceId = a.DeviceId,
                        DeviceName = a.Device.Name,
                        AlertType = a.AlertType,
                        Message = a.Message,
                        CreatedAt = a.CreatedAt,
                        ResolvedAt = a.ResolvedAt,
                        Severity = a.Severity,
                        Status = a.Status,
                        ImpactKW = a.ImpactKW,
                        EstimatedCost = a.EstimatedCost
                    })
                    .ToListAsync();

                return Ok(new { totalCount, pageNumber, pageSize, alerts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving alerts");
                return StatusCode(500, "Error retrieving alerts");
            }
        }

        /// <summary>
        /// Get critical alerts for a site
        /// </summary>
        [HttpGet("critical/site/{siteId}")]
        public async Task<IActionResult> GetCriticalAlerts(int siteId)
        {
            try
            {
                var alerts = await _alertService.GetCriticalAlerts(siteId);

                var dtos = alerts.Select(a => new AlertDto
                {
                    Id = a.Id,
                    DeviceId = a.DeviceId,
                    DeviceName = a.Device?.Name,
                    AlertType = a.AlertType,
                    Message = a.Message,
                    CreatedAt = a.CreatedAt,
                    Severity = a.Severity,
                    Status = a.Status,
                    ImpactKW = a.ImpactKW,
                    EstimatedCost = a.EstimatedCost
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving critical alerts for site {SiteId}", siteId);
                return StatusCode(500, "Error retrieving critical alerts");
            }
        }

        /// <summary>
        /// Get alert summary for a site
        /// </summary>
        [HttpGet("summary/site/{siteId}")]
        public async Task<IActionResult> GetAlertSummary(int siteId)
        {
            try
            {
                var (critical, high, medium, avgImpact) = await _alertService.GetAlertSummary(siteId);

                var totalCost = (critical * 15000) + (high * 5000) + (medium * 1000);

                var dto = new AlertSummaryDto
                {
                    CriticalAlerts = critical,
                    HighAlerts = high,
                    MediumAlerts = medium,
                    AverageImpactKW = avgImpact,
                    EstimatedTotalCost = totalCost
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving alert summary for site {SiteId}", siteId);
                return StatusCode(500, "Error retrieving summary");
            }
        }

        /// <summary>
        /// Get alerts for a specific device
        /// </summary>
        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetDeviceAlerts(int deviceId, [FromQuery] string status = "Open")
        {
            try
            {
                var query = _context.Alerts
                    .Where(a => a.DeviceId == deviceId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(a => a.Status == status);

                var alerts = await query
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(a => new AlertDto
                    {
                        Id = a.Id,
                        DeviceId = a.DeviceId,
                        AlertType = a.AlertType,
                        Message = a.Message,
                        CreatedAt = a.CreatedAt,
                        Severity = a.Severity,
                        Status = a.Status,
                        ImpactKW = a.ImpactKW,
                        EstimatedCost = a.EstimatedCost
                    })
                    .ToListAsync();

                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving alerts for device {DeviceId}", deviceId);
                return StatusCode(500, "Error retrieving device alerts");
            }
        }

        /// <summary>
        /// Create a manual alert
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var device = await _context.Devices.FindAsync(dto.DeviceId);
                if (device == null)
                    return NotFound("Device not found");

                var alert = new Alert
                {
                    DeviceId = dto.DeviceId,
                    AlertType = dto.AlertType,
                    Message = dto.Message,
                    Severity = dto.Severity,
                    Status = "Open",
                    CreatedAt = DateTime.UtcNow,
                    ImpactKW = dto.ImpactKW,
                    EstimatedCost = dto.ImpactKW.HasValue ? dto.ImpactKW.Value * 50 : null
                };

                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAlerts), new { id = alert.Id }, alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert");
                return StatusCode(500, "Error creating alert");
            }
        }

        /// <summary>
        /// Resolve an alert
        /// </summary>
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveAlert(int id)
        {
            try
            {
                await _alertService.ResolveAlert(id);
                return Ok(new { message = "Alert resolved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving alert {AlertId}", id);
                return StatusCode(500, "Error resolving alert");
            }
        }

        /// <summary>
        /// Update alert status
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateAlertStatus(int id, [FromBody] dynamic request)
        {
            try
            {
                var alert = await _context.Alerts.FindAsync(id);
                if (alert == null)
                    return NotFound();

                string status = request.status;
                if (string.IsNullOrEmpty(status))
                    return BadRequest("Status is required");

                alert.Status = status;
                if (status == "Resolved")
                    alert.ResolvedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating alert status {AlertId}", id);
                return StatusCode(500, "Error updating alert");
            }
        }

        /// <summary>
        /// Delete an alert
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlert(int id)
        {
            try
            {
                var alert = await _context.Alerts.FindAsync(id);
                if (alert == null)
                    return NotFound();

                _context.Alerts.Remove(alert);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting alert {AlertId}", id);
                return StatusCode(500, "Error deleting alert");
            }
        }
    }
}
