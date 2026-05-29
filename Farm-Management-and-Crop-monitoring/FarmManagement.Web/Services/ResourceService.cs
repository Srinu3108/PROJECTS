using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using FarmManagement.Web.Services.Strategies;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

// Strategy Pattern — allocation behaviour is injected via IAllocationStrategy.
// Swap StandardAllocationStrategy for ReserveAllocationStrategy in Program.cs
// without changing any code here.
public class ResourceService : IResourceService
{
    private readonly FarmDbContext       _db;
    private readonly IAllocationStrategy _allocationStrategy;

    public ResourceService(FarmDbContext db, IAllocationStrategy allocationStrategy)
    {
        _db                 = db;
        _allocationStrategy = allocationStrategy;
    }

    public async Task<IEnumerable<Resource>> GetAllAsync() =>
        await _db.Resources.AsNoTracking().OrderBy(r => r.Name).ToListAsync();

    public async Task<Resource?> GetByIdAsync(int id) =>
        await _db.Resources
                 .Include(r => r.ResourceUsages)
                     .ThenInclude(ru => ru.PlantingSchedule)
                         .ThenInclude(ps => ps.Crop)
                 .FirstOrDefaultAsync(r => r.ResourceId == id);

    public async Task CreateAsync(InventoryViewModel vm)
    {
        _db.Resources.Add(new Resource
        {
            Name        = vm.Name,
            Type        = vm.Type,
            Quantity    = vm.Quantity,
            Unit        = vm.Unit,
            LastUpdated = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public async Task CreateAsync(InventoryViewModel vm, int pestIncidentId)
    {
        _db.Resources.Add(new Resource
        {
            Name           = vm.Name,
            Type           = vm.Type,
            Quantity       = vm.Quantity,
            Unit           = vm.Unit,
            LastUpdated    = DateTime.Now,
            PestIncidentId = pestIncidentId
        });
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(InventoryViewModel vm)
    {
        var resource = await _db.Resources.FindAsync(vm.ResourceId)
                       ?? throw new KeyNotFoundException("Resource not found.");
        resource.Name        = vm.Name;
        resource.Type        = vm.Type;
        resource.Quantity    = vm.Quantity;
        resource.Unit        = vm.Unit;
        resource.LastUpdated = DateTime.Now;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var resource = await _db.Resources
            .Include(r => r.ResourceUsages)
            .FirstOrDefaultAsync(r => r.ResourceId == id);

        if (resource != null)
        {
            _db.ResourceUsages.RemoveRange(resource.ResourceUsages);
            _db.Resources.Remove(resource);
            await _db.SaveChangesAsync();
        }
    }

    public async Task AllocateAsync(int resourceId, int scheduleId, decimal qty, string? notes)
    {
        var resource = await _db.Resources.FindAsync(resourceId)
                       ?? throw new KeyNotFoundException("Resource not found.");

        // Strategy Pattern — delegates the allocation rule to the injected strategy
        await _allocationStrategy.AllocateAsync(resource, qty);

        resource.LastUpdated = DateTime.Now;

        _db.ResourceUsages.Add(new ResourceUsage
        {
            ResourceId   = resourceId,
            ScheduleId   = scheduleId,
            QuantityUsed = qty,
            Notes        = notes,
            UsedDate     = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Resource>> GetLowStockAsync(decimal threshold = 10) =>
        await _db.Resources.Where(r => r.Quantity <= threshold).ToListAsync();
}
