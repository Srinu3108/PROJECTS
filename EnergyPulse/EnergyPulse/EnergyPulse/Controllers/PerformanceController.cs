using Microsoft.AspNetCore.Mvc;
using EnergyPulse.DTOs;
using EnergyPulse.Services;

namespace EnergyPulse.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly PerformanceService _performanceService;
        private readonly ILogger<PerformanceController> _logger;

        public PerformanceController(PerformanceService performanceService, ILogger<PerformanceController> logger)
        {
            _performanceService = performanceService;
            _logger = logger;
        }

        /// <summary>
        /// Get performance trend for a device over the specified period
        /// </summary>
        [HttpGet("trends/device/{deviceId}")]
        public async Task<IActionResult> GetDeviceTrend(int deviceId, [FromQuery] int daysBack = 30)
        {
            try
            {
                var trend = await _performanceService.GetDevicePerformanceTrend(deviceId, daysBack);

                var dtos = trend.Select(t => new PerformanceTrendDto
                {
                    Date = t.Date,
                    EfficiencyPercentage = t.EfficiencyPercentage,
                    ActualOutput = t.ActualOutput,
                    TargetOutput = t.TargetOutput
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving device trend for device {DeviceId}", deviceId);
                return StatusCode(500, "Error retrieving trend data");
            }
        }

        /// <summary>
        /// Get comparative analysis of all devices in a site
        /// </summary>
        [HttpGet("comparison/site/{siteId}")]
        public async Task<IActionResult> GetSiteComparison(int siteId)
        {
            try
            {
                var comparison = await _performanceService.GetSiteDeviceComparison(siteId);

                var dtos = comparison.Select(c => new DeviceComparisonDto
                {
                    DeviceName = c.DeviceName,
                    CurrentEfficiency = c.CurrentEfficiency,
                    AverageEfficiency = c.AverageEfficiency,
                    OpenAlertCount = c.AlertCount,
                    LastReading = c.LastReading,
                    Status = $"Current: {c.CurrentEfficiency:F1}%, Avg: {c.AverageEfficiency:F1}%"
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving site comparison for site {SiteId}", siteId);
                return StatusCode(500, "Error retrieving comparison data");
            }
        }

        /// <summary>
        /// Calculate revenue loss due to underperformance
        /// </summary>
        [HttpGet("revenue-loss/site/{siteId}")]
        public async Task<IActionResult> GetRevenueLoss(int siteId, [FromQuery] double pricePerKWh = 50.0)
        {
            try
            {
                var (totalLossKW, estimatedCost, affectedDevices) =
                    await _performanceService.CalculateRevenueLoss(siteId, pricePerKWh);

                var dto = new RevenueLossDto
                {
                    TotalLossKW = totalLossKW,
                    EstimatedCostUSD = estimatedCost,
                    AffectedDevices = affectedDevices,
                    PeriodDays = "Last 24 hours"
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating revenue loss for site {SiteId}", siteId);
                return StatusCode(500, "Error calculating revenue loss");
            }
        }

        /// <summary>
        /// Get maintenance recommendations for a site
        /// </summary>
        [HttpGet("recommendations/site/{siteId}")]
        public async Task<IActionResult> GetMaintenanceRecommendations(int siteId)
        {
            try
            {
                var recommendations = await _performanceService.GetMaintenanceRecommendations(siteId);

                var dtos = recommendations.Select(r => new MaintenanceRecommendationDto
                {
                    DeviceName = r.DeviceName,
                    Recommendation = r.Recommendation,
                    Priority = r.Priority
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance recommendations for site {SiteId}", siteId);
                return StatusCode(500, "Error retrieving recommendations");
            }
        }

        /// <summary>
        /// Get device reliability score (0-100)
        /// </summary>
        [HttpGet("reliability-score/device/{deviceId}")]
        public async Task<IActionResult> GetReliabilityScore(int deviceId)
        {
            try
            {
                var score = await _performanceService.CalculateDeviceReliabilityScore(deviceId);
                return Ok(new { deviceId, reliabilityScore = score, grade = GetGrade(score) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating reliability score for device {DeviceId}", deviceId);
                return StatusCode(500, "Error calculating score");
            }
        }

        private string GetGrade(double score)
        {
            return score switch
            {
                >= 90 => "A (Excellent)",
                >= 80 => "B (Good)",
                >= 70 => "C (Fair)",
                >= 60 => "D (Poor)",
                _ => "F (Critical)"
            };
        }
    }
}
