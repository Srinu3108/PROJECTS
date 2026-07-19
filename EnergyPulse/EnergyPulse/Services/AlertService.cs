using EnergyPulse.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyPulse.Services
{
    public class AlertService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AlertService> _logger;

        public AlertService(AppDbContext context, ILogger<AlertService> logger = null)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Check power reading and create alerts if thresholds are exceeded
        /// </summary>
        public async Task CheckAndCreateAlert(PowerReading reading)
        {
            try
            {
                if (reading == null)
                    return;

                var device = await _context.Devices.FindAsync(reading.DeviceId);
                if (device == null)
                    return;

                var site = await _context.Sites.FindAsync(device.SiteId);
                if (site == null)
                    return;

                // Calculate efficiency
                var efficiency = reading.TargetOutputKW > 0
                    ? (reading.ActualOutputKW / reading.TargetOutputKW) * 100
                    : 0;

                // Rule 1: Device Offline (0 output)
                if (reading.ActualOutputKW == 0 && reading.TargetOutputKW > 0)
                {
                    await CreateAlertIfNotExists(reading.DeviceId, "Hardware", "Device is offline (0 output)",
                        "Critical", 100);
                }

                // Rule 2: Severely Low Efficiency (< 50%)
                if (efficiency < 50 && efficiency > 0)
                {
                    await CreateAlertIfNotExists(reading.DeviceId, "Performance",
                        $"Severely low efficiency detected: {efficiency:F2}%",
                        "Critical", reading.TargetOutputKW - reading.ActualOutputKW);
                }

                // Rule 3: Low Efficiency (50-70%)
                else if (efficiency < 70 && efficiency >= 50)
                {
                    await CreateAlertIfNotExists(reading.DeviceId, "Performance",
                        $"Low efficiency detected: {efficiency:F2}%",
                        "High", reading.TargetOutputKW - reading.ActualOutputKW);
                }

                // Rule 4: Below Site Target
                else if (efficiency < site.PerformanceTarget)
                {
                    await CreateAlertIfNotExists(reading.DeviceId, "Performance",
                        $"Device efficiency {efficiency:F2}% below site target {site.PerformanceTarget:F2}%",
                        "Medium", reading.TargetOutputKW - reading.ActualOutputKW);
                }

                // Rule 5: Sensor Issues (negative output)
                if (reading.ActualOutputKW < 0 || reading.TargetOutputKW < 0)
                {
                    await CreateAlertIfNotExists(reading.DeviceId, "Sensor",
                        "Invalid sensor readings detected (negative values)",
                        "High", 0);
                }

                // Rule 6: Sudden drop in output
                var previousReading = await _context.PowerReadings
                    .Where(r => r.DeviceId == reading.DeviceId && r.Id != reading.Id)
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefaultAsync();

                if (previousReading != null)
                {
                    var outputDrop = previousReading.ActualOutputKW - reading.ActualOutputKW;
                    var percentDrop = previousReading.ActualOutputKW > 0
                        ? (outputDrop / previousReading.ActualOutputKW) * 100
                        : 0;

                    if (percentDrop > 30 && reading.TargetOutputKW > 0) // 30% drop
                    {
                        await CreateAlertIfNotExists(reading.DeviceId, "Performance",
                            $"Sudden output drop detected: {percentDrop:F2}% decrease",
                            "High", outputDrop);
                    }
                }

                // Update device current efficiency
                device.CurrentEfficiency = efficiency;
                device.LastReadingDate = reading.Timestamp;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking and creating alert for power reading {ReadingId}", reading?.Id);
            }
        }

        /// <summary>
        /// Create an alert if a similar open alert doesn't already exist
        /// </summary>
        private async Task CreateAlertIfNotExists(int deviceId, string alertType, string message,
            string severity, double impactKW)
        {
            try
            {
                // Check if similar alert already exists in last hour
                var existingAlert = await _context.Alerts
                    .Where(a => a.DeviceId == deviceId &&
                                a.AlertType == alertType &&
                                a.Status == "Open" &&
                                a.CreatedAt > DateTime.UtcNow.AddHours(-1))
                    .FirstOrDefaultAsync();

                if (existingAlert == null)
                {
                    var alert = new Alert
                    {
                        DeviceId = deviceId,
                        AlertType = alertType,
                        Message = message,
                        CreatedAt = DateTime.UtcNow,
                        Severity = severity,
                        Status = "Open",
                        ImpactKW = impactKW > 0 ? impactKW : null,
                        EstimatedCost = impactKW > 0 ? impactKW * 50 : null // Assume $50 per KW loss
                    };

                    _context.Alerts.Add(alert);
                    await _context.SaveChangesAsync();

                    _logger?.LogInformation("Created new alert for device {DeviceId}: {AlertType} - {Message}",
                        deviceId, alertType, message);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating alert for device {DeviceId}", deviceId);
            }
        }

        /// <summary>
        /// Get all open critical alerts for a site
        /// </summary>
        public async Task<List<Alert>> GetCriticalAlerts(int siteId)
        {
            return await _context.Alerts
                .Where(a => a.Device.SiteId == siteId &&
                           a.Status == "Open" &&
                           a.Severity == "Critical")
                .Include(a => a.Device)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Resolve an alert
        /// </summary>
        public async Task ResolveAlert(int alertId, string notes = "")
        {
            var alert = await _context.Alerts.FindAsync(alertId);
            if (alert != null)
            {
                alert.Status = "Resolved";
                alert.ResolvedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger?.LogInformation("Resolved alert {AlertId}", alertId);
            }
        }

        /// <summary>
        /// Get performance summary for alert dashboard
        /// </summary>
        public async Task<(int CriticalAlerts, int HighAlerts, int MediumAlerts, double AverageImpactKW)>
            GetAlertSummary(int siteId)
        {
            var alerts = await _context.Alerts
                .Where(a => a.Device.SiteId == siteId && a.Status == "Open")
                .ToListAsync();

            var criticalCount = alerts.Count(a => a.Severity == "Critical");
            var highCount = alerts.Count(a => a.Severity == "High");
            var mediumCount = alerts.Count(a => a.Severity == "Medium");
            var avgImpact = alerts.Where(a => a.ImpactKW.HasValue).Average(a => a.ImpactKW ?? 0);

            return (criticalCount, highCount, mediumCount, avgImpact);
        }
    }
}
