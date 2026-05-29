using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.DTOs;

public class ResourceDto
{
    public int ResourceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }

    // Computed flag for dashboard low-stock alert
    public bool IsLowStock => Quantity <= 10;
}