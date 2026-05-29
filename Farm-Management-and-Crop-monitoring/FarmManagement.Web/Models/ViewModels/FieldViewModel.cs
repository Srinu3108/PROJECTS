using System.ComponentModel.DataAnnotations;

namespace FarmManagement.Web.Models.ViewModels;

public class FieldViewModel
{
    public int FieldId { get; set; }

    [Required(ErrorMessage = "Field name is required.")]
    [StringLength(100)]
    [Display(Name = "Field Name")]
    public string FieldName { get; set; } = string.Empty;

    [Required]
    [Range(0.1, 10000)]
    [Display(Name = "Area (Hectares)")]
    public decimal AreaHectares { get; set; }

    [Required(ErrorMessage = "Soil type is required.")]
    [Display(Name = "Soil Type")]
    public string SoilType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required.")]
    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;
}
