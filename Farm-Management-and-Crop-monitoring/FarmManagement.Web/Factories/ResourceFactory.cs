using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Factories;

// Factory Pattern — centralises all Resource <-> ViewModel mapping in one place
public class ResourceFactory : IResourceFactory
{
    public InventoryViewModel ToViewModel(Resource resource) => new InventoryViewModel
    {
        ResourceId = resource.ResourceId,
        Name       = resource.Name,
        Type       = resource.Type,
        Quantity   = resource.Quantity,
        Unit       = resource.Unit
    };

    public Resource ToEntity(InventoryViewModel vm) => new Resource
    {
        Name        = vm.Name,
        Type        = vm.Type,
        Quantity    = vm.Quantity,
        Unit        = vm.Unit,
        LastUpdated = DateTime.Now
    };
}
