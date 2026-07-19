using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Interfaces;

public interface IResourceService
{
    Task<IEnumerable<Resource>> GetAllAsync();
    Task<Resource?> GetByIdAsync(int id);
    Task CreateAsync(InventoryViewModel vm);
    Task CreateAsync(InventoryViewModel vm, int pestIncidentId);
    Task UpdateAsync(InventoryViewModel vm);
    Task DeleteAsync(int id);
    Task AllocateAsync(int resourceId, int scheduleId, decimal qty, string? notes); // fixed: fieldId → scheduleId
    Task<IEnumerable<Resource>> GetLowStockAsync(decimal threshold = 10);
}