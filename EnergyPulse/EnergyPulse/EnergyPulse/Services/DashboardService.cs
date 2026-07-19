using EnergyPulse.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EnergyPulse.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardData()
        {
            var totalDevices = await _context.Devices.CountAsync();

            var activeDevices = await _context.Devices
                .CountAsync(d => d.Status == "Active");

            var averageEfficiency = await _context.PowerReadings
                .AverageAsync(r => r.ActualOutputKW / r.TargetOutputKW * 100);

            var totalAlerts = await _context.Alerts.CountAsync();
            var highAlerts = await _context.Alerts
                .CountAsync(a => a.Severity == "High");

            var mediumAlerts = await _context.Alerts
                .CountAsync(a => a.Severity == "Medium");

            return new DashboardDto
            {
                TotalDevices = totalDevices,
                ActiveDevices = activeDevices,
                AverageEfficiency = averageEfficiency,
                SystemPerformanceIndex = averageEfficiency,
                TotalAlerts = totalAlerts,
                HighAlerts = highAlerts,
                MediumAlerts = mediumAlerts
            };
        }
    }
}
