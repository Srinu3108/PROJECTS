using FarmManagement.Web.Events;
using FarmManagement.Web.Models.Interfaces;

namespace FarmManagement.Web.Services;

// Facade Pattern — hides the complexity of coordinating multiple services.
// Controllers call one method here instead of making 2-3 separate service calls.
public class FarmFacade : IFarmFacade
{
    private readonly IResourceService _resourceService;
    private readonly IScheduleService _scheduleService;
    private readonly ICropService     _cropService;
    private readonly IFieldService    _fieldService;
    private readonly IPestService     _pestService;
    private readonly IEventDispatcher _dispatcher;

    public FarmFacade(
        IResourceService resourceService,
        IScheduleService scheduleService,
        ICropService     cropService,
        IFieldService    fieldService,
        IPestService     pestService,
        IEventDispatcher dispatcher)
    {
        _resourceService = resourceService;
        _scheduleService = scheduleService;
        _cropService     = cropService;
        _fieldService    = fieldService;
        _pestService     = pestService;
        _dispatcher      = dispatcher;
    }

    // Allocate: calls ResourceService + dispatches ResourceAllocatedEvent
    public async Task AllocateResourceAsync(int resourceId, int scheduleId, decimal qty,
        string? notes, int userId, string userName, string role)
    {
        await _resourceService.AllocateAsync(resourceId, scheduleId, qty, notes);
        var resource = await _resourceService.GetByIdAsync(resourceId);
        await _dispatcher.DispatchAsync(new ResourceAllocatedEvent(
            userId, userName, role, resource!.Name, qty, resource.Unit, scheduleId));
    }

    // RecordHarvest: calls ScheduleService + dispatches HarvestRecordedEvent
    public async Task RecordHarvestAsync(int scheduleId, decimal actualYield,
        string? notes, int userId, string userName, string role)
    {
        await _scheduleService.RecordHarvestAsync(scheduleId, actualYield, notes);
        await _dispatcher.DispatchAsync(new HarvestRecordedEvent(
            userId, userName, role, scheduleId, actualYield));
    }

    // DeleteCrop: calls CropService + dispatches CropDeletedEvent
    public async Task DeleteCropAsync(int cropId, string cropName,
        int userId, string userName, string role)
    {
        await _cropService.DeleteAsync(cropId);
        await _dispatcher.DispatchAsync(new CropDeletedEvent(userId, userName, role, cropName));
    }

    // DeleteField: calls FieldService + dispatches FieldDeletedEvent
    public async Task DeleteFieldAsync(int fieldId, string fieldName,
        int userId, string userName, string role)
    {
        await _fieldService.DeleteAsync(fieldId);
        await _dispatcher.DispatchAsync(new FieldDeletedEvent(userId, userName, role, fieldName));
    }

    // DeleteResource: calls ResourceService + dispatches ResourceDeletedEvent
    public async Task DeleteResourceAsync(int resourceId, string resourceName,
        int userId, string userName, string role)
    {
        await _resourceService.DeleteAsync(resourceId);
        await _dispatcher.DispatchAsync(new ResourceDeletedEvent(userId, userName, role, resourceName));
    }

    // DeletePest: calls PestService + dispatches PestDeletedEvent
    public async Task DeletePestAsync(int pestId, string pestName,
        int userId, string userName, string role)
    {
        await _pestService.DeleteAsync(pestId);
        await _dispatcher.DispatchAsync(new PestDeletedEvent(userId, userName, role, pestName));
    }

    // DeleteSchedule: calls ScheduleService + dispatches ScheduleDeletedEvent
    public async Task DeleteScheduleAsync(int scheduleId,
        int userId, string userName, string role)
    {
        await _scheduleService.DeleteAsync(scheduleId);
        await _dispatcher.DispatchAsync(new ScheduleDeletedEvent(userId, userName, role, scheduleId));
    }
}
