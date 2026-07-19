using Microsoft.AspNetCore.Mvc;
using EnergyPulse.Data;
using EnergyPulse.DTOs;
using EnergyPulse.Models;
using EnergyPulse.Services;
using Microsoft.EntityFrameworkCore;

namespace EnergyPulse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(AppDbContext context, ReportService reportService, ILogger<ReportController> logger)
        {
            _context = context;
            _reportService = reportService;
            _logger = logger;
        }

        /// <summary>
        /// Get summary report for a site
        /// </summary>
        [HttpGet("summary/{siteId}")]
        public async Task<IActionResult> GetReportSummary(int siteId)
        {
            try
            {
                var siteName = await _context.Sites
                    .Where(s => s.Id == siteId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync() ?? "Solar Plant Alpha";

                var totalDevices = await _context.Devices
                    .Where(d => d.SiteId == siteId)
                    .CountAsync();

                var activeDevices = await _context.Devices
                    .Where(d => d.SiteId == siteId && d.Status == "Active")
                    .CountAsync();

                // ✅ FIXED: Get readings and calculate average in memory
                var readings = await _context.PowerReadings
                    .Where(r => r.Device.SiteId == siteId)
                    .Select(p => new { p.ActualOutputKW, p.TargetOutputKW })
                    .ToListAsync();

                var avgEfficiency = readings.Any()
                    ? readings.Average(p => (p.ActualOutputKW / p.TargetOutputKW) * 100)
                    : 0;

                var totalAlerts = await _context.Alerts
                    .Where(a => a.Device.SiteId == siteId)
                    .CountAsync();

                var highAlerts = await _context.Alerts
                    .Where(a => a.Device.SiteId == siteId && a.Severity == "High")
                    .CountAsync();

                // Get devices with their latest reading
                var devicesWithReadings = await _context.Devices
                    .Where(d => d.SiteId == siteId)
                    .Select(d => new
                    {
                        d.Id,
                        d.Name,
                        d.Status,
                        LatestReading = _context.PowerReadings
                            .Where(p => p.DeviceId == d.Id)
                            .OrderByDescending(p => p.Timestamp)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                var underperformingDevices = devicesWithReadings
                    .Where(d => d.Status == "Fault" ||
                                (d.LatestReading != null &&
                                d.LatestReading.TargetOutputKW > 0 &&
                                (d.LatestReading.ActualOutputKW / d.LatestReading.TargetOutputKW * 100) < 80))
                    .Select(d => new DeviceIssueDto
                    {
                        DeviceName = d.Name,
                        Status = d.Status,
                        PerformanceIndex = d.LatestReading != null && d.LatestReading.TargetOutputKW > 0
                            ? Math.Round((d.LatestReading.ActualOutputKW / d.LatestReading.TargetOutputKW) * 100, 0)
                            : 0,
                        Issue = d.Status == "Fault" ? "Device is in fault state" : "Efficiency below 80% target"
                    })
                    .OrderBy(dto => dto.PerformanceIndex)
                    .ToList();

                var report = new ReportDto
                {
                    Site = siteName,
                    TotalDevices = totalDevices,
                    ActiveDevices = activeDevices,
                    AverageEfficiency = Math.Round(avgEfficiency, 2),
                    SystemPerformanceIndex = Math.Round(avgEfficiency, 2),
                    TotalAlerts = totalAlerts,
                    HighAlerts = highAlerts,
                    GeneratedAt = DateTime.UtcNow,
                    UnderperformingDevices = underperformingDevices
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report summary for site {SiteId}", siteId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Generate PDF maintenance report for a site
        /// </summary>
        [HttpGet("pdf/maintenance/{siteId}")]
        public async Task<IActionResult> GenerateMaintenancePdf(int siteId)
        {
            try
            {
                var pdfBytes = await _reportService.GenerateMaintenanceReportPdfAsync(siteId);
                return File(pdfBytes, "application/pdf", $"MaintenanceReport-Site{siteId}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF report for site {SiteId}", siteId);
                return StatusCode(500, "Error generating report");
            }
        }

        /// <summary>
        /// Generate PDF report for a specific device
        /// </summary>
        [HttpGet("pdf/device/{deviceId}")]
        public async Task<IActionResult> GenerateDevicePdf(int deviceId)
        {
            try
            {
                var pdfBytes = await _reportService.GenerateDeviceReportPdfAsync(deviceId);
                return File(pdfBytes, "application/pdf", $"DeviceReport-Device{deviceId}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating device PDF report for device {DeviceId}", deviceId);
                return StatusCode(500, "Error generating report");
            }
        }

        /// <summary>
        /// Get all maintenance records
        /// </summary>
        [HttpGet("maintenance-records")]
        public async Task<IActionResult> GetMaintenanceRecords([FromQuery] int? siteId = null, [FromQuery] string status = null)
        {
            try
            {
                var query = _context.MaintenanceRecords
                    .Include(m => m.Device)
                    .Include(m => m.TechnicianAssigned)
                    .AsQueryable();

                if (siteId.HasValue)
                    query = query.Where(m => m.Device.SiteId == siteId);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(m => m.Status == status);

                var records = await query
                    .OrderByDescending(m => m.ReportDate)
                    .Select(m => new MaintenanceRecordDto
                    {
                        Id = m.Id,
                        DeviceId = m.DeviceId,
                        DeviceName = m.Device.Name,
                        Title = m.Title,
                        Description = m.Description,
                        ReportDate = m.ReportDate,
                        CompletedDate = m.CompletedDate,
                        Status = m.Status,
                        Priority = m.Priority,
                        EstimatedCost = m.EstimatedCost,
                        ActualCost = m.ActualCost,
                        Notes = m.Notes
                    })
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance records");
                return StatusCode(500, "Error retrieving records");
            }
        }

        /// <summary>
        /// Create a new maintenance record
        /// </summary>
        [HttpPost("maintenance-records")]
        public async Task<IActionResult> CreateMaintenanceRecord([FromBody] CreateMaintenanceRecordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var device = await _context.Devices.FindAsync(dto.DeviceId);
                if (device == null)
                    return NotFound("Device not found");

                var record = new MaintenanceRecord
                {
                    DeviceId = dto.DeviceId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Priority = dto.Priority,
                    EstimatedCost = dto.EstimatedCost,
                    Status = "Pending",
                    ReportDate = DateTime.UtcNow
                };

                _context.MaintenanceRecords.Add(record);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMaintenanceRecords), new { id = record.Id }, record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating maintenance record");
                return StatusCode(500, "Error creating record");
            }
        }

        /// <summary>
        /// Update a maintenance record
        /// </summary>
        [HttpPut("maintenance-records/{id}")]
        public async Task<IActionResult> UpdateMaintenanceRecord(int id, [FromBody] UpdateMaintenanceRecordDto dto)
        {
            try
            {
                var record = await _context.MaintenanceRecords.FindAsync(id);
                if (record == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(dto.Title))
                    record.Title = dto.Title;
                if (!string.IsNullOrEmpty(dto.Description))
                    record.Description = dto.Description;
                if (!string.IsNullOrEmpty(dto.Status))
                    record.Status = dto.Status;
                if (!string.IsNullOrEmpty(dto.Priority))
                    record.Priority = dto.Priority;
                if (dto.ActualCost.HasValue)
                    record.ActualCost = dto.ActualCost;
                if (!string.IsNullOrEmpty(dto.Notes))
                    record.Notes = dto.Notes;

                if (dto.Status == "Completed" && !record.CompletedDate.HasValue)
                    record.CompletedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance record {RecordId}", id);
                return StatusCode(500, "Error updating record");
            }
        }

        /// <summary>
        /// Delete a maintenance record
        /// </summary>
        [HttpDelete("maintenance-records/{id}")]
        public async Task<IActionResult> DeleteMaintenanceRecord(int id)
        {
            try
            {
                var record = await _context.MaintenanceRecords.FindAsync(id);
                if (record == null)
                    return NotFound();

                _context.MaintenanceRecords.Remove(record);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting maintenance record {RecordId}", id);
                return StatusCode(500, "Error deleting record");
            }
        }
    }
}