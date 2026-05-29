using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

// Template Method Pattern — extends BaseEntityService which defines the Create skeleton.
// CropService implements BuildEntity (required) and ValidateViewModel (optional hook).
public class CropService : BaseEntityService<Crop, CropViewModel>, ICropService
{
    public CropService(FarmDbContext db) : base(db) { }

    // ── Template Method hooks ────────────────────────────────────────────────

    // Step 1 hook — custom business-rule validation before saving
    protected override void ValidateViewModel(CropViewModel vm)
    {
        if (vm.PlantingDate >= vm.ExpectedHarvestDate)
            throw new ArgumentException("Planting date must be before the expected harvest date.");
    }

    // Step 2 — constructs the Crop entity from the ViewModel
    protected override Crop BuildEntity(CropViewModel vm) => new Crop
    {
        CropName            = vm.CropName,
        CropType            = vm.CropType,
        Season              = vm.Season,
        PlantingDate        = vm.PlantingDate,
        ExpectedHarvestDate = vm.ExpectedHarvestDate,
        FieldId             = vm.FieldId,
        Status              = "Growing"
    };

    // ── ICropService implementation ──────────────────────────────────────────

    public async Task<IEnumerable<Crop>> GetAllAsync() =>
        await _db.Crops.AsNoTracking()
                       .Include(c => c.Field)
                       .OrderByDescending(c => c.CropId)
                       .ToListAsync();

    public async Task<Crop?> GetByIdAsync(int id) =>
        await _db.Crops.AsNoTracking()
                       .Include(c => c.Field)
                       .Include(c => c.PestIncidents)
                       .Include(c => c.PlantingSchedules)
                       .FirstOrDefaultAsync(c => c.CropId == id);

    // Delegates to the base template method — Validate → BuildEntity → Save → AfterCreate
    public async Task CreateAsync(CropViewModel vm) => await TemplateCreateAsync(vm);

    public async Task UpdateAsync(CropViewModel vm)
    {
        var crop = await _db.Crops.FindAsync(vm.CropId)
                   ?? throw new KeyNotFoundException("Crop not found.");
        crop.CropName            = vm.CropName;
        crop.CropType            = vm.CropType;
        crop.Season              = vm.Season;
        crop.PlantingDate        = vm.PlantingDate;
        crop.ExpectedHarvestDate = vm.ExpectedHarvestDate;
        crop.FieldId             = vm.FieldId;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var crop = await _db.Crops.FindAsync(id);
        if (crop != null)
        {
            var scheduleIds = await _db.PlantingSchedules
                .Where(ps => ps.CropId == id)
                .Select(ps => ps.ScheduleId)
                .ToListAsync();

            if (scheduleIds.Count > 0)
            {
                var usages = _db.ResourceUsages.Where(r => scheduleIds.Contains(r.ScheduleId));
                _db.ResourceUsages.RemoveRange(usages);

                var harvests = _db.Harvests.Where(h => scheduleIds.Contains(h.ScheduleId));
                _db.Harvests.RemoveRange(harvests);

                var schedules = _db.PlantingSchedules.Where(ps => scheduleIds.Contains(ps.ScheduleId));
                _db.PlantingSchedules.RemoveRange(schedules);
            }

            var pests = _db.PestIncidents.Where(p => p.CropId == id);
            _db.PestIncidents.RemoveRange(pests);

            var yieldReports = _db.YieldReports.Where(y => y.CropId == id);
            _db.YieldReports.RemoveRange(yieldReports);

            _db.Crops.Remove(crop);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<CropViewModel> PrepareViewModelAsync(CropViewModel? vm = null)
    {
        vm ??= new CropViewModel();
        var fields = await _db.Fields.OrderBy(f => f.FieldName).ToListAsync();
        vm.Fields = fields.Select(f => new SelectListItem
        {
            Value    = f.FieldId.ToString(),
            Text     = $"{f.FieldName} ({f.Location})",
            Selected = f.FieldId == vm.FieldId
        }).ToList();
        return vm;
    }
}
