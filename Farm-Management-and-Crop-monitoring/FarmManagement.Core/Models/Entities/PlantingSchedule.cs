using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Models.Entities;

public class PlantingSchedule
{
    public int ScheduleId { get; set; }
    public int CropId { get; set; }
    public int FieldId { get; set; }
    public DateTime ScheduledDate { get; set; }
    [Precision(18, 2)]
    public decimal ExpectedYieldKg { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string? Notes { get; set; }

    public Crop Crop { get; set; } = null!;
    public Field Field { get; set; } = null!;
    public ICollection<Harvest> Harvests { get; set; } = new List<Harvest>();
    public ICollection<ResourceUsage> ResourceUsages { get; set; } = new List<ResourceUsage>();
}
