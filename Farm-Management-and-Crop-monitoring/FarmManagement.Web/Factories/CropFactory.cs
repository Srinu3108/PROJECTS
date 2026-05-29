using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Factories;

// Factory Pattern — centralises all Crop <-> ViewModel mapping in one place
public class CropFactory : ICropFactory
{
    public CropViewModel ToViewModel(Crop crop) => new CropViewModel
    {
        CropId              = crop.CropId,
        CropName            = crop.CropName,
        CropType            = crop.CropType,
        Season              = crop.Season,
        PlantingDate        = crop.PlantingDate,
        ExpectedHarvestDate = crop.ExpectedHarvestDate,
        FieldId             = crop.FieldId,
        Status              = crop.Status
    };

    public Crop ToEntity(CropViewModel vm) => new Crop
    {
        CropName            = vm.CropName,
        CropType            = vm.CropType,
        Season              = vm.Season,
        PlantingDate        = vm.PlantingDate,
        ExpectedHarvestDate = vm.ExpectedHarvestDate,
        FieldId             = vm.FieldId,
        Status              = "Growing"
    };
}
