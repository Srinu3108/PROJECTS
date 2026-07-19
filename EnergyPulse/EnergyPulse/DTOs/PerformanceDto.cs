namespace EnergyPulse.DTOs
{
    public class DeviceDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DeviceType { get; set; }
        public string Status { get; set; }
        public double Capacity { get; set; }
        public double CurrentEfficiency { get; set; }
        public DateTime InstalledDate { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime LastReadingDate { get; set; }

        public int OpenAlertCount { get; set; }
        public int CriticalAlertCount { get; set; }
        public double ReliabilityScore { get; set; }
        public double AverageEfficiency { get; set; }

        public List<PowerReadingDto> RecentReadings { get; set; } = new();
        public List<AlertDto> OpenAlerts { get; set; } = new();
    }

    public class PowerReadingDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double ActualOutputKW { get; set; }
        public double TargetOutputKW { get; set; }
        public double Efficiency { get; set; }
    }

    public class PerformanceTrendDto
    {
        public DateTime Date { get; set; }
        public double EfficiencyPercentage { get; set; }
        public double ActualOutput { get; set; }
        public double TargetOutput { get; set; }
    }

    public class DeviceComparisonDto
    {
        public string DeviceName { get; set; }
        public double CurrentEfficiency { get; set; }
        public double AverageEfficiency { get; set; }
        public int OpenAlertCount { get; set; }
        public DateTime LastReading { get; set; }
        public string Status { get; set; }
    }

    public class RevenueLossDto
    {
        public double TotalLossKW { get; set; }
        public double EstimatedCostUSD { get; set; }
        public int AffectedDevices { get; set; }
        public string PeriodDays { get; set; } = "Last 24 hours";
    }

    public class MaintenanceRecommendationDto
    {
        public string DeviceName { get; set; }
        public string Recommendation { get; set; }
        public string Priority { get; set; }
    }
}
