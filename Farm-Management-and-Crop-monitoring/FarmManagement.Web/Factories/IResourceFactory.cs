using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Factories;

public interface IResourceFactory
{
    InventoryViewModel ToViewModel(Resource resource);
    Resource ToEntity(InventoryViewModel vm);
}
