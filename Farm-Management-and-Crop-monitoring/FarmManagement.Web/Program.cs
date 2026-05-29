using FarmManagement.Web.Data;
using FarmManagement.Web.Events;
using FarmManagement.Web.Events.Handlers;
using FarmManagement.Web.Factories;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.Validations;
using FarmManagement.Web.Services;
using FarmManagement.Web.Services.Strategies;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<FarmDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("FarmManagement.Web")));

builder.Services.AddControllersWithViews();

// ── Validation ────────────────────────────────────────────────────────────────
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CropValidator>();

// ── Authentication ────────────────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath       = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan  = TimeSpan.FromHours(8);
    });

// ── Caching (Decorator Pattern) ───────────────────────────────────────────────
builder.Services.AddMemoryCache();

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IFieldService,          FieldService>();
builder.Services.AddScoped<IPestService,           PestService>();
builder.Services.AddScoped<IScheduleService,       ScheduleService>();
builder.Services.AddScoped<IReportService,         ReportService>();
builder.Services.AddScoped<IAccountService,        AccountService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IActivityService,       ActivityService>();

// Decorator Pattern — register the real CropService by its concrete type,
// then register ICropService as CachedCropService (which wraps the real one).
builder.Services.AddScoped<CropService>();
builder.Services.AddScoped<ICropService>(provider =>
    new CachedCropService(
        provider.GetRequiredService<CropService>(),
        provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()));

// Strategy Pattern — swap to ReserveAllocationStrategy here to change allocation rules
builder.Services.AddScoped<IAllocationStrategy, StandardAllocationStrategy>();
builder.Services.AddScoped<IResourceService,    ResourceService>();

// ── Factory Pattern ───────────────────────────────────────────────────────────
builder.Services.AddScoped<ICropFactory,      CropFactory>();
builder.Services.AddScoped<IFieldFactory,     FieldFactory>();
builder.Services.AddScoped<IResourceFactory,  ResourceFactory>();

// ── Observer Pattern — Event Dispatcher ──────────────────────────────────────
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

// Crop event handlers
builder.Services.AddScoped<IEventHandler<CropCreatedEvent>,  CropCreatedHandler>();
builder.Services.AddScoped<IEventHandler<CropUpdatedEvent>,  CropUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<CropDeletedEvent>,  CropDeletedHandler>();

// Field event handlers
builder.Services.AddScoped<IEventHandler<FieldCreatedEvent>, FieldCreatedHandler>();
builder.Services.AddScoped<IEventHandler<FieldUpdatedEvent>, FieldUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<FieldDeletedEvent>, FieldDeletedHandler>();

// Resource event handlers
builder.Services.AddScoped<IEventHandler<ResourceCreatedEvent>,   ResourceCreatedHandler>();
builder.Services.AddScoped<IEventHandler<ResourceUpdatedEvent>,   ResourceUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<ResourceAllocatedEvent>, ResourceAllocatedHandler>();
builder.Services.AddScoped<IEventHandler<ResourceDeletedEvent>,   ResourceDeletedHandler>();

// Pest event handlers
builder.Services.AddScoped<IEventHandler<PestReportedEvent>,      PestReportedHandler>();
builder.Services.AddScoped<IEventHandler<PestStatusUpdatedEvent>, PestStatusUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<PestDeletedEvent>,       PestDeletedHandler>();

// Schedule / Harvest event handlers
builder.Services.AddScoped<IEventHandler<ScheduleCreatedEvent>,  ScheduleCreatedHandler>();
builder.Services.AddScoped<IEventHandler<HarvestRecordedEvent>,  HarvestRecordedHandler>();
builder.Services.AddScoped<IEventHandler<ScheduleDeletedEvent>,  ScheduleDeletedHandler>();

// ── Facade Pattern ────────────────────────────────────────────────────────────
builder.Services.AddScoped<IFarmFacade, FarmFacade>();

// ── Singleton Pattern — one shared cache instance for the entire app ──────────
builder.Services.AddSingleton(FarmCacheService.Instance);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<FarmDbContext>();
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying database migrations.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
