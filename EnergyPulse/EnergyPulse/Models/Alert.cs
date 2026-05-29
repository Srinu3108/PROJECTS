namespace EnergyPulse.Models
{
    public class Alert
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }

        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string Severity { get; set; } = null!;
    }
}
