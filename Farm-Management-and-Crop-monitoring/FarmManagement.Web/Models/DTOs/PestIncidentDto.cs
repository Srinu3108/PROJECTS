using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.DTOs;

public class PestIncidentDto
{
    public int PestIncidentId { get; set; }
    public string PestName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReportedDate { get; set; }
    public IncidentStatus Status { get; set; }
    public string? DiseaseName { get; set; }

    // Related crop info (flattened — no navigation property)
    public int CropId { get; set; }
    public string? CropName { get; set; }
}