namespace EnergyPulse.Models
{
    public class Alert
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public Device? Device { get; set; }

        public string Message { get; set; } = null!;
        public string AlertType { get; set; } = "Performance"; // Performance, Hardware, Sensor, etc.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

        public string Severity { get; set; } = "Medium"; // Low, Medium, High, Critical
        public string Status { get; set; } = "Open"; // Open, In Review, Resolved, Ignored

        public double? ImpactKW { get; set; } // Estimated power loss in KW
        public double? EstimatedCost { get; set; } // Estimated revenue loss
    }
}
