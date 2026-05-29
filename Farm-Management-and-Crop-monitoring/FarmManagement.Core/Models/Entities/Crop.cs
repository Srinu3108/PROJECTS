using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.Entities;

public class Crop
{
    public int CropId { get; set; }
    public string CropName { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty;
    public SeasonType Season { get; set; }
    public DateTime PlantingDate { get; set; }
    public DateTime ExpectedHarvestDate { get; set; }
    public string Status { get; set; } = "Growing";
    public int FieldId { get; set; }

    public Field Field { get; set; } = null!;
    public ICollection<PestIncident> PestIncidents { get; set; } = new List<PestIncident>();
    public ICollection<PlantingSchedule> PlantingSchedules { get; set; } = new List<PlantingSchedule>();
    public ICollection<YieldReport> YieldReports { get; set; } = new List<YieldReport>();
}
