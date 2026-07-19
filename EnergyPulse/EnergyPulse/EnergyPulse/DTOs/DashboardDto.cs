namespace EnergyPulse.DTOs
{
    public class DashboardDto
    {
        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }
        public double AverageEfficiency { get; set; }
        public double SystemPerformanceIndex { get; set; }
        public int TotalAlerts { get; set; }

        public int HighAlerts { get; set; }
        public int MediumAlerts { get; set; }
    }
}
