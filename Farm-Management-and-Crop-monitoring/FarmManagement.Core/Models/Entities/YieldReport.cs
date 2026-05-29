using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.Entities;

public class YieldReport
{
    public int YieldReportId { get; set; }
    public int CropId { get; set; }
    public decimal TotalYieldKg { get; set; }
    public decimal AverageYieldPerAcre { get; set; }
    public SeasonType Season { get; set; }
    public int Year { get; set; }
    public string? Remarks { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.Now;

    public Crop Crop { get; set; } = null!;
}
