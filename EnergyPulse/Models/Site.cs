namespace EnergyPulse.Models
{
    public class Site
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = string.Empty;
        public string Type { get; set; } = "Solar"; // Solar, Wind, Hydro, etc.

        public double TotalCapacityKW { get; set; }
        public DateTime EstablishedDate { get; set; }

        public List<Device> Devices { get; set; } = new();
        public List<User> Users { get; set; } = new();

        public double PerformanceTarget { get; set; } = 85.0; // Target performance percentage
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
