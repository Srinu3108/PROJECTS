namespace FarmManagement.Web.Models.DTOs;

public class HarvestDto
{
    public int HarvestId { get; set; }
    public DateTime HarvestedDate { get; set; }
    public decimal ActualYieldKg { get; set; }
    public string? Notes { get; set; }

    // Related schedule info (flattened — no navigation property)
    public int ScheduleId { get; set; }

    // Related crop info (flattened — no navigation property)
    public string? CropName { get; set; }
    public string? CropType { get; set; }

    // Related field info (flattened — no navigation property)
    public string? FieldName { get; set; }
}