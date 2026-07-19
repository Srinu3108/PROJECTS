namespace EnergyPulse.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = "Technician"; // Admin or Technician
        public int? SiteId { get; set; }

        public Site? Site { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        public List<MaintenanceRecord> CreatedReports { get; set; } = new();
    }
}
