namespace EnergyPulse.Models
{
    public class MaintenanceRecord
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int? UserId { get; set; }
        public int? AlertId { get; set; }

        public Device? Device { get; set; }
        public User? TechnicianAssigned { get; set; }
        public Alert? RelatedAlert { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime ReportDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedDate { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        public double? EstimatedCost { get; set; }
        public double? ActualCost { get; set; }

        public string Notes { get; set; } = string.Empty;
    }
}
