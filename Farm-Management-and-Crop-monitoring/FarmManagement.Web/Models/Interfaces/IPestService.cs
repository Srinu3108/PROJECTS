using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Models.Interfaces;

public interface IPestService
{
    Task<IEnumerable<PestIncident>> GetAllAsync();
    Task<PestIncident?> GetByIdAsync(int id);
    Task<IEnumerable<PestIncident>> GetActivesAsync();
    Task CreateAsync(PestIncident incident);
    Task UpdateStatusAsync(int id, string status);
    Task UpdateAsync(PestIncident incident);
    Task DeleteAsync(int id);
}