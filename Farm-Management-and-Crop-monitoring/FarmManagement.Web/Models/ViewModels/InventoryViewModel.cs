using FarmManagement.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FarmManagement.Web.Models.ViewModels;

public class InventoryViewModel
{
    public int ResourceId { get; set; }

    [Required(ErrorMessage = "Resource name is required.")]
    [StringLength(100)]
    [Display(Name = "Resource Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Type")]
    public ResourceType Type { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    [Display(Name = "Quantity")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    [Display(Name = "Unit (kg / L / units)")]
    public string Unit { get; set; } = string.Empty;

    // ── Pest Incident fields (used only when Type == Pesticide) ──

    [Display(Name = "Pest Name")]
    public string? PestName { get; set; }

    [Display(Name = "Disease Name")]
    public string? DiseaseName { get; set; }

    [Display(Name = "Description")]
    public string? PestDescription { get; set; }

    [Display(Name = "Affected Crop")]
    public int? CropId { get; set; }
}