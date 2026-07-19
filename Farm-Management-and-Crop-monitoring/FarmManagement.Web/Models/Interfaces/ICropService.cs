using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Interfaces;

public interface ICropService
{
    Task<IEnumerable<Crop>> GetAllAsync();
    Task<Crop?> GetByIdAsync(int id);
    Task CreateAsync(CropViewModel vm);
    Task UpdateAsync(CropViewModel vm);
    Task DeleteAsync(int id);
    Task<CropViewModel> PrepareViewModelAsync(CropViewModel? vm = null);
}