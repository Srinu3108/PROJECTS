namespace EnergyPulse.Api.DTOs
{
    public class DeviceStatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Efficiency { get; set; }
        public string Type { get; set; } = "Solar Inverter";
    }
}
