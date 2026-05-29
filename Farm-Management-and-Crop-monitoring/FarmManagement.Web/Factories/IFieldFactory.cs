using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Factories;

public interface IFieldFactory
{
    FieldViewModel ToViewModel(Field field);
    Field ToEntity(FieldViewModel vm);
}
