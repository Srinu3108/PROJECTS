namespace EnergyPulse.DTOs
{
    public class AlertDto
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public double? ImpactKW { get; set; }
        public double? EstimatedCost { get; set; }
    }

    public class CreateAlertDto
    {
        public int DeviceId { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; } = "Medium";
        public double? ImpactKW { get; set; }
    }

    public class AlertSummaryDto
    {
        public int CriticalAlerts { get; set; }
        public int HighAlerts { get; set; }
        public int MediumAlerts { get; set; }
        public int TotalOpenAlerts => CriticalAlerts + HighAlerts + MediumAlerts;
        public double AverageImpactKW { get; set; }
        public double EstimatedTotalCost { get; set; }
    }
}
