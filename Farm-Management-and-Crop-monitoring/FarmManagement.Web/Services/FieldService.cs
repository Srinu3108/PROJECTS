using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class FieldService : IFieldService
{
    private readonly FarmDbContext _db;
    public FieldService(FarmDbContext db) => _db = db;

    public async Task<IEnumerable<Field>> GetAllAsync() =>
        await _db.Fields.AsNoTracking()
                        .Include(f => f.Crops)
                        .OrderByDescending(f => f.FieldId)
                        .ToListAsync();

    public async Task<Field?> GetByIdAsync(int id) =>
        await _db.Fields.AsNoTracking()
                        .Include(f => f.Crops)
                        .Include(f => f.PlantingSchedules)
                        .FirstOrDefaultAsync(f => f.FieldId == id);

    public async Task CreateAsync(FieldViewModel vm)
    {
        _db.Fields.Add(new Field
        {
            FieldName = vm.FieldName,
            AreaHectares = vm.AreaHectares,
            SoilType = vm.SoilType,
            Location = vm.Location,
            CreatedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(FieldViewModel vm)
    {
        var field = await _db.Fields.FindAsync(vm.FieldId)
                    ?? throw new KeyNotFoundException("Field not found.");
        field.FieldName    = vm.FieldName;
        field.AreaHectares = vm.AreaHectares;
        field.SoilType     = vm.SoilType;
        field.Location     = vm.Location;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var field = await _db.Fields
            .Include(f => f.Crops).ThenInclude(c => c.PestIncidents)
            .Include(f => f.Crops).ThenInclude(c => c.PlantingSchedules).ThenInclude(ps => ps.Harvests)
            .Include(f => f.Crops).ThenInclude(c => c.PlantingSchedules).ThenInclude(ps => ps.ResourceUsages)
            .Include(f => f.Crops).ThenInclude(c => c.YieldReports)
            .Include(f => f.PlantingSchedules).ThenInclude(ps => ps.Harvests)
            .Include(f => f.PlantingSchedules).ThenInclude(ps => ps.ResourceUsages)
            .FirstOrDefaultAsync(f => f.FieldId == id);

        if (field == null) return;

        // Remove child records bottom-up to avoid FK constraint errors
        foreach (var crop in field.Crops)
        {
            _db.PestIncidents.RemoveRange(crop.PestIncidents);
            _db.YieldReports.RemoveRange(crop.YieldReports);
            foreach (var ps in crop.PlantingSchedules)
            {
                _db.Harvests.RemoveRange(ps.Harvests);
                _db.ResourceUsages.RemoveRange(ps.ResourceUsages);
            }
            _db.PlantingSchedules.RemoveRange(crop.PlantingSchedules);
        }
        _db.Crops.RemoveRange(field.Crops);

        foreach (var ps in field.PlantingSchedules)
        {
            _db.Harvests.RemoveRange(ps.Harvests);
            _db.ResourceUsages.RemoveRange(ps.ResourceUsages);
        }
        _db.PlantingSchedules.RemoveRange(field.PlantingSchedules);

        _db.Fields.Remove(field);
        await _db.SaveChangesAsync();
    }
}
