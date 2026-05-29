using FarmManagement.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FarmManagement.Web.Models.ViewModels;

public class CropViewModel
{
    public int CropId { get; set; }

    [Required(ErrorMessage = "Crop name is required.")]
    [StringLength(100)]
    [Display(Name = "Crop Name")]
    public string CropName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Crop type is required.")]
    [Display(Name = "Crop Type")]
    public string CropType { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Season")]
    public SeasonType Season { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Planting Date")]
    public DateTime PlantingDate { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Expected Harvest Date")]
    public DateTime ExpectedHarvestDate { get; set; } = DateTime.Today.AddMonths(3);

    [Required(ErrorMessage = "Please select a field.")]
    [Display(Name = "Field")]
    public int FieldId { get; set; }

    public string? Status { get; set; }

    public IEnumerable<SelectListItem> Fields { get; set; } = new List<SelectListItem>();
}
