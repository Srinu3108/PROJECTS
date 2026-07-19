namespace EnergyPulse.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = "Active"; // Active, Inactive, Maintenance, Fault
        public string DeviceType { get; set; } = "Solar Panel"; // Solar Panel, Inverter, Battery, etc.

        public int SiteId { get; set; }
        public Site? Site { get; set; }

        public double Capacity { get; set; } = 0.0; // Capacity in KW
        public DateTime InstalledDate { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }

        public double CurrentEfficiency { get; set; } = 0.0; // Latest efficiency percentage
        public DateTime LastReadingDate { get; set; }

        public List<PowerReading> Readings { get; set; } = new();
        public List<Alert> Alerts { get; set; } = new();
        public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new();
    }
}
