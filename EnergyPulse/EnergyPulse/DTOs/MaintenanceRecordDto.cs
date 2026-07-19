namespace EnergyPulse.DTOs
{
    public class MaintenanceRecordDto
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; } // Pending, In Progress, Completed
        public string Priority { get; set; } // Low, Medium, High, Critical
        public double? EstimatedCost { get; set; }
        public double? ActualCost { get; set; }
        public string Notes { get; set; }
    }

    public class CreateMaintenanceRecordDto
    {
        public int DeviceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; } = "Medium";
        public double? EstimatedCost { get; set; }
    }

    public class UpdateMaintenanceRecordDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public double? ActualCost { get; set; }
        public string Notes { get; set; }
    }
}
