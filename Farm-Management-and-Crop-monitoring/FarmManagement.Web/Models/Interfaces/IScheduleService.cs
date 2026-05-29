using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Interfaces;

public interface IScheduleService
{
    Task<IEnumerable<PlantingSchedule>> GetAllAsync();
    Task<PlantingSchedule?> GetByIdAsync(int id);
    Task CreateAsync(ScheduleViewModel vm);
    Task UpdateAsync(ScheduleViewModel vm);
    Task DeleteAsync(int id);
    Task RecordHarvestAsync(int scheduleId, decimal actualYield, string? notes);
    Task<IEnumerable<PlantingSchedule>> GetUpcomingAsync(int days = 30);
    Task<ScheduleViewModel> PrepareViewModelAsync(ScheduleViewModel? vm = null);
}