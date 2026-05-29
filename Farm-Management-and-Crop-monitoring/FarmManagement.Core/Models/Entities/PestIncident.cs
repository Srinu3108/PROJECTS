using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.Entities;

public class PestIncident
{
    public int PestIncidentId { get; set; }
    public string PestName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; } = IncidentStatus.Active;
    public DateTime ReportedDate { get; set; } = DateTime.Now;
    public string? DiseaseName { get; set; }
    public int CropId { get; set; }

    public Crop Crop { get; set; } = null!;

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
