namespace EnergyPulse.Api.DTOs
{
    public class CreateDeviceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int SiteId { get; set; }
    }
}
