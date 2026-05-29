namespace FarmManagement.Web.Models.DTOs;

public class FieldDto
{
    public int FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public decimal AreaHectares { get; set; }
    public string? SoilType { get; set; }
    public string? Location { get; set; }

    // Summary counts (useful for display)
    public int TotalCrops { get; set; }
    public int TotalSchedules { get; set; }
}