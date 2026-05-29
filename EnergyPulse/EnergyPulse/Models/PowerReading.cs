namespace EnergyPulse.Models
{
    public class PowerReading
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public Device? Device { get; set; }

        public DateTime Timestamp { get; set; }

        public double ActualOutputKW { get; set; }
        public double TargetOutputKW { get; set; }

        public double Efficiency => (ActualOutputKW / TargetOutputKW) * 100;
    }
}
