using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FarmManagement.Web.Models.ViewModels;

public class ScheduleViewModel
{
    public int ScheduleId { get; set; }

    [Required(ErrorMessage = "Please select a crop.")]
    [Display(Name = "Crop")]
    public int CropId { get; set; }

    [Required(ErrorMessage = "Please select a field.")]
    [Display(Name = "Field")]
    public int FieldId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Scheduled Harvest Date")]
    public DateTime ScheduledDate { get; set; } = DateTime.Today.AddDays(7);

    [Required]
    [Range(0.1, 100000)]
    [Display(Name = "Expected Yield (kg)")]
    public decimal ExpectedYieldKg { get; set; }

    [Display(Name = "Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    public string? Status { get; set; }
    public string? CropName { get; set; }
    public string? FieldName { get; set; }

    public IEnumerable<SelectListItem> Crops { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Fields { get; set; } = new List<SelectListItem>();

    public List<ResourceUsageItem> ResourceUsages { get; set; } = new();
}

public class ResourceUsageItem
{
    public int ResourceId { get; set; }
    public decimal QuantityUsed { get; set; }
    public string? Notes { get; set; }
}
