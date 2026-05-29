using FarmManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

// Template Method Pattern — defines the skeleton of the Create operation.
// Subclasses fill in the specific steps (Validate, BuildEntity, AfterCreate)
// without changing the overall algorithm structure.
public abstract class BaseEntityService<TEntity, TViewModel>
    where TEntity : class
{
    protected readonly FarmDbContext _db;

    protected BaseEntityService(FarmDbContext db) => _db = db;

    // ── Template Method ───────────────────────────────────────────────────────
    // Defines the fixed Create algorithm: Validate → Build → Save → AfterCreate
    protected async Task TemplateCreateAsync(TViewModel vm)
    {
        ValidateViewModel(vm);                       // Step 1 — hook (optional)
        var entity = BuildEntity(vm);                // Step 2 — abstract (required)
        await _db.Set<TEntity>().AddAsync(entity);
        await _db.SaveChangesAsync();                // Step 3 — fixed
        await AfterCreateAsync(entity);              // Step 4 — hook (optional)
    }

    // Subclasses override this to add custom validation rules
    protected virtual void ValidateViewModel(TViewModel vm) { }

    // Subclasses must implement this to convert ViewModel → Entity
    protected abstract TEntity BuildEntity(TViewModel vm);

    // Subclasses override this for post-create side effects (e.g. notifications)
    protected virtual Task AfterCreateAsync(TEntity entity) => Task.CompletedTask;
}
