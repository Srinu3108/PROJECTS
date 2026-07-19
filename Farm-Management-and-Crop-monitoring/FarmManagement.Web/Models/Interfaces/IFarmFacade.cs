namespace FarmManagement.Web.Models.Interfaces;

// Facade Pattern — single entry point for complex multi-service operations
public interface IFarmFacade
{
    Task AllocateResourceAsync(int resourceId, int scheduleId, decimal qty, string? notes,
                               int userId, string userName, string role);

    Task RecordHarvestAsync(int scheduleId, decimal actualYield, string? notes,
                            int userId, string userName, string role);

    Task DeleteCropAsync(int cropId, string cropName,
                         int userId, string userName, string role);

    Task DeleteFieldAsync(int fieldId, string fieldName,
                          int userId, string userName, string role);

    Task DeleteResourceAsync(int resourceId, string resourceName,
                             int userId, string userName, string role);

    Task DeletePestAsync(int pestId, string pestName,
                         int userId, string userName, string role);

    Task DeleteScheduleAsync(int scheduleId,
                             int userId, string userName, string role);
}
