using FarmManagement.Web.Models.Interfaces;

namespace FarmManagement.Web.Events.Handlers;

// Observer Pattern — each handler listens to one event and writes the activity log.
// Controllers no longer call IActivityService directly; they just dispatch an event.

// ── Crop Handlers ─────────────────────────────────────────────────────────────
public class CropCreatedHandler : IEventHandler<CropCreatedEvent>
{
    private readonly IActivityService _activity;
    public CropCreatedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(CropCreatedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Created", "Crop",
            $"Added crop '{e.CropName}' ({e.CropType}) for {e.Season} season");
}

public class CropUpdatedHandler : IEventHandler<CropUpdatedEvent>
{
    private readonly IActivityService _activity;
    public CropUpdatedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(CropUpdatedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Updated", "Crop",
            $"Updated crop '{e.CropName}' — status: {e.Status}");
}

public class CropDeletedHandler : IEventHandler<CropDeletedEvent>
{
    private readonly IActivityService _activity;
    public CropDeletedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(CropDeletedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Deleted", "Crop",
            $"Deleted crop '{e.CropName}'");
}

// ── Field Handlers ────────────────────────────────────────────────────────────
public class FieldCreatedHandler : IEventHandler<FieldCreatedEvent>
{
    private readonly IActivityService _activity;
    public FieldCreatedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(FieldCreatedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Created", "Field",
            $"Added field '{e.FieldName}' ({e.Area} ha, {e.SoilType})");
}

public class FieldUpdatedHandler : IEventHandler<FieldUpdatedEvent>
{
    private readonly IActivityService _activity;
    public FieldUpdatedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(FieldUpdatedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Updated", "Field",
            $"Updated field '{e.FieldName}' — {e.Area} ha, {e.SoilType}");
}

public class FieldDeletedHandler : IEventHandler<FieldDeletedEvent>
{
    private readonly IActivityService _activity;
    public FieldDeletedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(FieldDeletedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Deleted", "Field",
            $"Deleted field '{e.FieldName}'");
}

// ── Resource Handlers ─────────────────────────────────────────────────────────
public class ResourceCreatedHandler : IEventHandler<ResourceCreatedEvent>
{
    private readonly IActivityService _activity;
    public ResourceCreatedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(ResourceCreatedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Created", "Resource",
            $"Added resource '{e.Name}' — {e.Quantity} {e.Unit}");
}

public class ResourceUpdatedHandler : IEventHandler<ResourceUpdatedEvent>
{
    private readonly IActivityService _activity;
    public ResourceUpdatedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(ResourceUpdatedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Updated", "Resource",
            $"Updated resource '{e.Name}' — {e.Quantity} {e.Unit}");
}

public class ResourceAllocatedHandler : IEventHandler<ResourceAllocatedEvent>
{
    private readonly IActivityService _activity;
    public ResourceAllocatedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(ResourceAllocatedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Allocated", "Resource",
            $"Used {e.Quantity} {e.Unit} of '{e.Name}' for schedule #{e.ScheduleId}");
}

public class ResourceDeletedHandler : IEventHandler<ResourceDeletedEvent>
{
    private readonly IActivityService _activity;
    public ResourceDeletedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(ResourceDeletedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Deleted", "Resource",
            $"Deleted resource '{e.Name}'");
}

// ── Pest Handlers ─────────────────────────────────────────────────────────────
public class PestReportedHandler : IEventHandler<PestReportedEvent>
{
    private readonly IActivityService _activity;
    public PestReportedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(PestReportedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Reported", "Pest",
            $"Logged pest incident: '{e.PestName}'");
}

public class PestStatusUpdatedHandler : IEventHandler<PestStatusUpdatedEvent>
{
    private readonly IActivityService _activity;
    public PestStatusUpdatedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(PestStatusUpdatedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Updated", "Pest",
            $"Updated '{e.PestName}' status to {e.NewStatus}");
}

public class PestDeletedHandler : IEventHandler<PestDeletedEvent>
{
    private readonly IActivityService _activity;
    public PestDeletedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(PestDeletedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Deleted", "Pest",
            $"Deleted pest incident: '{e.PestName}'");
}

// ── Schedule / Harvest Handlers ───────────────────────────────────────────────
public class ScheduleCreatedHandler : IEventHandler<ScheduleCreatedEvent>
{
    private readonly IActivityService _activity;
    public ScheduleCreatedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(ScheduleCreatedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Created", "Schedule",
            $"Scheduled harvest for {e.ScheduledDate:dd MMM yyyy} — expected {e.ExpectedYield} kg");
}

public class HarvestRecordedHandler : IEventHandler<HarvestRecordedEvent>
{
    private readonly IActivityService _activity;
    public HarvestRecordedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(HarvestRecordedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Harvested", "Harvest",
            $"Recorded harvest #{e.ScheduleId} — actual yield: {e.ActualYield:N0} kg");
}

public class ScheduleDeletedHandler : IEventHandler<ScheduleDeletedEvent>
{
    private readonly IActivityService _activity;
    public ScheduleDeletedHandler(IActivityService activity) => _activity = activity;

    public Task HandleAsync(ScheduleDeletedEvent e) =>
        _activity.LogAsync(e.UserId, e.UserName, e.Role,
            "Deleted", "Schedule",
            $"Deleted schedule #{e.ScheduleId}");
}
