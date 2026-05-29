using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Factories;

public interface ICropFactory
{
    CropViewModel ToViewModel(Crop crop);
    Crop ToEntity(CropViewModel vm);
}
