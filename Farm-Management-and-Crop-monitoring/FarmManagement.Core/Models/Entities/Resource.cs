using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.Entities;

public class Resource
{
    public int ResourceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public int? PestIncidentId { get; set; }
    public PestIncident? PestIncident { get; set; }

    public ICollection<ResourceUsage> ResourceUsages { get; set; } = new List<ResourceUsage>();
}
