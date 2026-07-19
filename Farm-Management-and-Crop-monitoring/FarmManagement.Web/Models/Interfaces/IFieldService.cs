using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Interfaces;

public interface IFieldService
{
    Task<IEnumerable<Field>> GetAllAsync();
    Task<Field?> GetByIdAsync(int id);
    Task CreateAsync(FieldViewModel vm);
    Task UpdateAsync(FieldViewModel vm);
    Task DeleteAsync(int id);
}