namespace EnergyPulse.Models
{
    public class Site
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public double TotalCapacityKW { get; set; }

        public List<Device> Devices { get; set; } = new();
    }
}
