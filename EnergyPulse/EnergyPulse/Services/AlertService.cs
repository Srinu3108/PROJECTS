using EnergyPulse.Models;

namespace EnergyPulse.Services
{
    public class AlertService
    {
        private readonly AppDbContext _context;

        public AlertService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CheckAndCreateAlert(PowerReading reading)
        {
            // Rule 1: Device Offline
            if (reading.ActualOutputKW == 0)
            {
                _context.Alerts.Add(new Alert
                {
                    DeviceId = reading.DeviceId,
                    Message = "Device is offline (0 output)",
                    CreatedAt = DateTime.Now,
                    Severity = "High"
                });
            }

            // Rule 2: Low Efficiency
            var efficiency = (reading.ActualOutputKW / reading.TargetOutputKW) * 100;

            if (efficiency < 60)
            {
                _context.Alerts.Add(new Alert
                {
                    DeviceId = reading.DeviceId,
                    Message = $"Low efficiency detected: {efficiency:F2}%",
                    CreatedAt = DateTime.Now,
                    Severity = "Medium"
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
