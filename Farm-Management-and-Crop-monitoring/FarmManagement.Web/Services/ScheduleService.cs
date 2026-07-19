using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class ScheduleService : IScheduleService
{
    private readonly FarmDbContext _db;
    public ScheduleService(FarmDbContext db) => _db = db;

    public async Task<IEnumerable<PlantingSchedule>> GetAllAsync() =>
        await _db.PlantingSchedules.AsNoTracking()
                 .Include(ps => ps.Crop)
                 .Include(ps => ps.Field)
                 .OrderBy(ps => ps.ScheduledDate)
                 .ToListAsync();

    public async Task<PlantingSchedule?> GetByIdAsync(int id) =>
        await _db.PlantingSchedules.AsNoTracking()
                 .Include(ps => ps.Crop)
                     .ThenInclude(c => c.PestIncidents)
                 .Include(ps => ps.Field)
                 .Include(ps => ps.Harvests)
                 .Include(ps => ps.ResourceUsages)
                     .ThenInclude(ru => ru.Resource)
                 .FirstOrDefaultAsync(ps => ps.ScheduleId == id);

    public async Task CreateAsync(ScheduleViewModel vm)
    {
        var schedule = new PlantingSchedule
        {
            CropId = vm.CropId,
            FieldId = vm.FieldId,
            ScheduledDate = vm.ScheduledDate,
            ExpectedYieldKg = vm.ExpectedYieldKg,
            Notes = vm.Notes,
            Status = "Scheduled"
        };
        _db.PlantingSchedules.Add(schedule);
        await _db.SaveChangesAsync();

        if (vm.ResourceUsages != null && vm.ResourceUsages.Count > 0)
        {
            foreach (var ru in vm.ResourceUsages)
            {
                if (ru.ResourceId > 0 && ru.QuantityUsed > 0)
                {
                    var resource = await _db.Resources.FindAsync(ru.ResourceId);
                    if (resource != null)
                    {
                        resource.Quantity -= ru.QuantityUsed;
                        resource.LastUpdated = DateTime.Now;
                    }

                    _db.ResourceUsages.Add(new ResourceUsage
                    {
                        ResourceId   = ru.ResourceId,
                        ScheduleId   = schedule.ScheduleId,
                        QuantityUsed = ru.QuantityUsed,
                        Notes        = ru.Notes,
                        UsedDate     = DateTime.Now
                    });
                }
            }
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(ScheduleViewModel vm)
    {
        var ps = await _db.PlantingSchedules.FindAsync(vm.ScheduleId)
                 ?? throw new KeyNotFoundException("Schedule not found.");
        ps.CropId = vm.CropId;
        ps.FieldId = vm.FieldId;
        ps.ScheduledDate = vm.ScheduledDate;
        ps.ExpectedYieldKg = vm.ExpectedYieldKg;
        ps.Notes = vm.Notes;

        // Remove old resource usages and restore quantities
        var oldUsages = await _db.ResourceUsages
            .Where(ru => ru.ScheduleId == vm.ScheduleId)
            .ToListAsync();

        foreach (var old in oldUsages)
        {
            var resource = await _db.Resources.FindAsync(old.ResourceId);
            if (resource != null)
            {
                resource.Quantity += old.QuantityUsed;
                resource.LastUpdated = DateTime.Now;
            }
        }
        _db.ResourceUsages.RemoveRange(oldUsages);

        // Add new resource usages and deduct quantities
        if (vm.ResourceUsages != null && vm.ResourceUsages.Count > 0)
        {
            foreach (var ru in vm.ResourceUsages)
            {
                if (ru.ResourceId > 0 && ru.QuantityUsed > 0)
                {
                    var resource = await _db.Resources.FindAsync(ru.ResourceId);
                    if (resource != null)
                    {
                        resource.Quantity -= ru.QuantityUsed;
                        resource.LastUpdated = DateTime.Now;
                    }

                    _db.ResourceUsages.Add(new ResourceUsage
                    {
                        ResourceId   = ru.ResourceId,
                        ScheduleId   = vm.ScheduleId,
                        QuantityUsed = ru.QuantityUsed,
                        Notes        = ru.Notes,
                        UsedDate     = DateTime.Now
                    });
                }
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var ps = await _db.PlantingSchedules.FindAsync(id);
        if (ps != null)
        {
            var relatedUsages = _db.ResourceUsages.Where(r => r.ScheduleId == id);
            _db.ResourceUsages.RemoveRange(relatedUsages);
            _db.PlantingSchedules.Remove(ps);
            await _db.SaveChangesAsync();
        }
    }

    public async Task RecordHarvestAsync(int scheduleId, decimal actualYield, string? notes)
    {
        var ps = await _db.PlantingSchedules.FindAsync(scheduleId)
                 ?? throw new KeyNotFoundException("Schedule not found.");
        ps.Status = "Completed";
        _db.Harvests.Add(new Harvest
        {
            ScheduleId = scheduleId,
            ActualYieldKg = actualYield,
            Notes = notes,
            HarvestedDate = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<PlantingSchedule>> GetUpcomingAsync(int days = 30) =>
        await _db.PlantingSchedules
                 .Include(ps => ps.Crop)
                 .Include(ps => ps.Field)
                 .Where(ps => ps.ScheduledDate >= DateTime.Today
                           && ps.ScheduledDate <= DateTime.Today.AddDays(days)
                           && ps.Status == "Scheduled")
                 .OrderBy(ps => ps.ScheduledDate)
                 .ToListAsync();

    public async Task<ScheduleViewModel> PrepareViewModelAsync(ScheduleViewModel? vm = null)
    {
        vm ??= new ScheduleViewModel();

        var crops = await _db.Crops.Where(c => c.Status == "Growing")
                                   .OrderBy(c => c.CropName)
                                   .ToListAsync();
        vm.Crops = crops.Select(c => new SelectListItem
        {
            Value = c.CropId.ToString(),
            Text = c.CropName,
            Selected = c.CropId == vm.CropId
        }).ToList();

        var fields = await _db.Fields.OrderBy(f => f.FieldName).ToListAsync();
        vm.Fields = fields.Select(f => new SelectListItem
        {
            Value = f.FieldId.ToString(),
            Text = $"{f.FieldName} ({f.Location})",
            Selected = f.FieldId == vm.FieldId
        }).ToList();

        return vm;
    }
}
