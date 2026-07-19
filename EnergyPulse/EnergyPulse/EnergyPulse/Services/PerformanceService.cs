using EnergyPulse.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyPulse.Services
{
    public class PerformanceService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PerformanceService> _logger;

        public PerformanceService(AppDbContext context, ILogger<PerformanceService> logger = null)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get performance trend for a device over the specified period
        /// </summary>
        public async Task<List<(DateTime Date, double EfficiencyPercentage, double ActualOutput, double TargetOutput)>>
            GetDevicePerformanceTrend(int deviceId, int daysBack = 30)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysBack);

                var readings = await _context.PowerReadings
                    .Where(r => r.DeviceId == deviceId && r.Timestamp >= cutoffDate)
                    .OrderBy(r => r.Timestamp)
                    .ToListAsync();

                var dailyStats = readings
                    .GroupBy(r => r.Timestamp.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        AvgActual = g.Average(r => r.ActualOutputKW),
                        AvgTarget = g.Average(r => r.TargetOutputKW),
                        AvgEfficiency = g.Average(r => r.TargetOutputKW > 0 ? (r.ActualOutputKW / r.TargetOutputKW) * 100 : 0)
                    })
                    .ToList();

                return dailyStats
                    .Select(d => (d.Date, d.AvgEfficiency, d.AvgActual, d.AvgTarget))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting performance trend for device {DeviceId}", deviceId);
                return new List<(DateTime, double, double, double)>();
            }
        }

        /// <summary>
        /// Get comparative performance analysis across all devices in a site
        /// </summary>
        public async Task<List<(string DeviceName, double CurrentEfficiency, double AverageEfficiency,
            int AlertCount, DateTime LastReading)>>
            GetSiteDeviceComparison(int siteId)
        {
            try
            {
                var devices = await _context.Devices
                    .Where(d => d.SiteId == siteId)
                    .Include(d => d.Readings)
                    .Include(d => d.Alerts)
                    .ToListAsync();

                var comparison = new List<(string, double, double, int, DateTime)>();

                foreach (var device in devices)
                {
                    var currentEfficiency = device.CurrentEfficiency;
                    var avgEfficiency = device.Readings.Any()
                        ? device.Readings.Average(r => (r.ActualOutputKW / r.TargetOutputKW) * 100)
                        : 0;
                    var openAlerts = device.Alerts.Count(a => a.Status == "Open");
                    var lastReading = device.LastReadingDate;

                    comparison.Add((device.Name, currentEfficiency, avgEfficiency, openAlerts, lastReading));
                }

                return comparison.OrderByDescending(c => c.Item4).ThenBy(c => c.Item2).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting device comparison for site {SiteId}", siteId);
                return new List<(string, double, double, int, DateTime)>();
            }
        }

        /// <summary>
        /// Calculate estimated revenue loss due to underperformance
        /// </summary>
        public async Task<(double TotalLossKW, double EstimatedCostUSD, int AffectedDevices)>
            CalculateRevenueLoss(int siteId, double pricePerKWh = 50.0)
        {
            try
            {
                var recentReadings = await _context.PowerReadings
                    .Where(r => r.Device.SiteId == siteId &&
                               r.Timestamp >= DateTime.UtcNow.AddDays(-1))
                    .ToListAsync();

                var totalExpectedKW = recentReadings.Sum(r => r.TargetOutputKW);
                var totalActualKW = recentReadings.Sum(r => r.ActualOutputKW);
                var totalLossKW = totalExpectedKW - totalActualKW;
                var estimatedCost = totalLossKW * pricePerKWh;

                var affectedDevices = recentReadings
                    .GroupBy(r => r.DeviceId)
                    .Where(g => g.Average(r => (r.ActualOutputKW / r.TargetOutputKW) * 100) < 90)
                    .Count();

                return (totalLossKW, estimatedCost, affectedDevices);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error calculating revenue loss for site {SiteId}", siteId);
                return (0, 0, 0);
            }
        }

        /// <summary>
        /// Get predictive maintenance recommendations based on performance trends
        /// </summary>
        public async Task<List<(string DeviceName, string Recommendation, string Priority)>>
            GetMaintenanceRecommendations(int siteId)
        {
            try
            {
                var devices = await _context.Devices
                    .Where(d => d.SiteId == siteId)
                    .Include(d => d.Readings)
                    .Include(d => d.Alerts)
                    .ToListAsync();

                var recommendations = new List<(string, string, string)>();

                foreach (var device in devices)
                {
                    // Trend analysis: if last week efficiency is worse than previous week
                    var lastWeekReadings = device.Readings
                        .Where(r => r.Timestamp >= DateTime.UtcNow.AddDays(-7))
                        .ToList();

                    var previousWeekReadings = device.Readings
                        .Where(r => r.Timestamp >= DateTime.UtcNow.AddDays(-14) &&
                                   r.Timestamp < DateTime.UtcNow.AddDays(-7))
                        .ToList();

                    if (lastWeekReadings.Any() && previousWeekReadings.Any())
                    {
                        var lastWeekEfficiency = lastWeekReadings.Average(r =>
                            (r.ActualOutputKW / r.TargetOutputKW) * 100);
                        var previousWeekEfficiency = previousWeekReadings.Average(r =>
                            (r.ActualOutputKW / r.TargetOutputKW) * 100);

                        var efficiencyDrop = previousWeekEfficiency - lastWeekEfficiency;

                        if (efficiencyDrop > 10)
                        {
                            recommendations.Add((device.Name,
                                $"Efficiency dropped {efficiencyDrop:F1}% week-over-week. Schedule inspection.",
                                "High"));
                        }
                    }

                    // Maintenance age check
                    if (device.LastMaintenanceDate.HasValue)
                    {
                        var daysSinceLastMaintenance = (DateTime.UtcNow - device.LastMaintenanceDate.Value).TotalDays;

                        if (daysSinceLastMaintenance > 180)
                        {
                            recommendations.Add((device.Name,
                                $"Last maintenance was {daysSinceLastMaintenance:F0} days ago. Schedule preventive maintenance.",
                                "Medium"));
                        }
                    }

                    // High alert count
                    var openAlertCount = device.Alerts.Count(a => a.Status == "Open");
                    if (openAlertCount > 5)
                    {
                        recommendations.Add((device.Name,
                            $"{openAlertCount} open alerts. Immediate investigation recommended.",
                            "Critical"));
                    }
                }

                return recommendations.OrderByDescending(r =>
                    r.Item3 == "Critical" ? 3 : r.Item3 == "High" ? 2 : 1).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting maintenance recommendations for site {SiteId}", siteId);
                return new List<(string, string, string)>();
            }
        }

        /// <summary>
        /// Get device reliability score (0-100)
        /// </summary>
        public async Task<double> CalculateDeviceReliabilityScore(int deviceId)
        {
            try
            {
                var device = await _context.Devices
                    .Include(d => d.Readings)
                    .Include(d => d.Alerts)
                    .FirstOrDefaultAsync(d => d.Id == deviceId);

                if (device == null)
                    return 0;

                double score = 100;

                // Efficiency impact
                var avgEfficiency = device.Readings.Any()
                    ? device.Readings.Average(r => (r.ActualOutputKW / r.TargetOutputKW) * 100)
                    : 100;

                if (avgEfficiency < 90)
                    score -= (90 - avgEfficiency) * 0.5;

                // Alert impact
                var criticalAlerts = device.Alerts.Count(a => a.Severity == "Critical" && a.Status == "Open");
                var highAlerts = device.Alerts.Count(a => a.Severity == "High" && a.Status == "Open");

                score -= criticalAlerts * 15;
                score -= highAlerts * 8;

                // Maintenance currency
                if (device.LastMaintenanceDate.HasValue)
                {
                    var daysSinceMaintenance = (DateTime.UtcNow - device.LastMaintenanceDate.Value).TotalDays;
                    if (daysSinceMaintenance > 180)
                        score -= (daysSinceMaintenance - 180) / 10; // -1 point per 10 days over 180
                }

                return Math.Max(0, Math.Min(100, score));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error calculating reliability score for device {DeviceId}", deviceId);
                return 0;
            }
        }
    }
}
