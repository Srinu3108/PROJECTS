using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Factories;

// Factory Pattern — centralises all Field <-> ViewModel mapping in one place
public class FieldFactory : IFieldFactory
{
    public FieldViewModel ToViewModel(Field field) => new FieldViewModel
    {
        FieldId      = field.FieldId,
        FieldName    = field.FieldName,
        AreaHectares = field.AreaHectares,
        SoilType     = field.SoilType,
        Location     = field.Location
    };

    public Field ToEntity(FieldViewModel vm) => new Field
    {
        FieldName    = vm.FieldName,
        AreaHectares = vm.AreaHectares,
        SoilType     = vm.SoilType,
        Location     = vm.Location,
        CreatedAt    = DateTime.Now
    };
}
