namespace EnergyPulse.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;

        public int SiteId { get; set; }
        public Site? Site { get; set; }

        public List<PowerReading> Readings { get; set; } = new();
    }
}
