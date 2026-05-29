namespace FarmManagement.Web.Models.DTOs;

public class ScheduleDto
{
    public int ScheduleId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Related crop info (flattened — no navigation property)
    public int CropId { get; set; }
    public string? CropName { get; set; }
    public string? CropType { get; set; }

    // Related field info (flattened — no navigation property)
    public int FieldId { get; set; }
    public string? FieldName { get; set; }

    // Computed flag for dashboard upcoming harvest alert
    public bool IsUpcoming => ScheduledDate >= DateTime.Today
                           && ScheduledDate <= DateTime.Today.AddDays(30)
                           && Status == "Scheduled";
}