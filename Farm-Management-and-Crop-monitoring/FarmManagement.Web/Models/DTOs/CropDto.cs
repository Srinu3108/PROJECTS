using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.DTOs;

public class CropDto
{
    public int CropId { get; set; }
    public string CropName { get; set; } = string.Empty;
    public string? CropType { get; set; }
    public SeasonType Season { get; set; }        // fixed: enum not string
    public DateTime PlantingDate { get; set; }
    public DateTime ExpectedHarvestDate { get; set; }
    public string? Status { get; set; }

    // Related field info (flattened)
    public int FieldId { get; set; }
    public string? FieldName { get; set; }
}