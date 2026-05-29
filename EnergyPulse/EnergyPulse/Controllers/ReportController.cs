using Microsoft.AspNetCore.Mvc;
using EnergyPulse.Data;
using EnergyPulse.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EnergyPulse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/report
        [HttpGet]
        public IActionResult GetReport()
        {
            var siteName = _context.Sites.Select(s => s.Name).FirstOrDefault() ?? "Solar Plant Alpha";
            var totalDevices = _context.Devices.Count();
            var activeDevices = _context.Devices.Count(d => d.Status == "Active");

            var avgEfficiency = _context.PowerReadings.Any()
                ? _context.PowerReadings.Average(p => (p.ActualOutputKW / p.TargetOutputKW) * 100)
                : 0;

            var totalAlerts = _context.Alerts.Count();
            var highAlerts = _context.Alerts.Count(a => a.Severity == "High");

            var latestReadings = _context.PowerReadings
                .OrderByDescending(r => r.Timestamp)
                .AsEnumerable()
                .GroupBy(r => r.DeviceId)
                .ToDictionary(g => g.Key, g => g.First());

            var underperformingDevices = _context.Devices
                .AsEnumerable()
                .Select(device =>
                {
                    latestReadings.TryGetValue(device.Id, out var latestReading);
                    var performanceIndex = 0.0;
                    var issue = "No recent reading";

                    if (latestReading != null && latestReading.TargetOutputKW > 0)
                    {
                        performanceIndex = (latestReading.ActualOutputKW / latestReading.TargetOutputKW) * 100;
                        issue = performanceIndex < 80
                            ? $"Underperforming at {performanceIndex:F0}%"
                            : "Monitoring";
                    }

                    if (device.Status == "Offline")
                    {
                        performanceIndex = 0;
                        issue = "Device is offline";
                    }

                    return new DeviceIssueDto
                    {
                        DeviceName = device.Name,
                        Status = device.Status,
                        PerformanceIndex = Math.Round(performanceIndex, 0),
                        Issue = issue
                    };
                })
                .Where(dto => dto.Status == "Offline" || dto.PerformanceIndex < 80)
                .OrderBy(dto => dto.PerformanceIndex)
                .ToList();

            var report = new ReportDto
            {
                Site = siteName,
                TotalDevices = totalDevices,
                ActiveDevices = activeDevices,
                AverageEfficiency = avgEfficiency,
                SystemPerformanceIndex = avgEfficiency,
                TotalAlerts = totalAlerts,
                HighAlerts = highAlerts,
                GeneratedAt = DateTime.Now,
                UnderperformingDevices = underperformingDevices
            };

            return Ok(report);
        }
    }
}