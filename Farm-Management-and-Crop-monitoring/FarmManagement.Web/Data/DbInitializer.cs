using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(FarmDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.AppUsers.AnyAsync())
        {
            context.AppUsers.Add(new AppUser
            {
                FullName = "System Administrator",
                Email = "admin@farmmanagement.com",
                PasswordHash = PasswordHelper.Hash("Admin@123"),
                Role = UserRole.Admin,
                CreatedAt = DateTime.Now,
                PasswordHint = "Default admin hint"
            });
            await context.SaveChangesAsync();
        }

        if (!await context.Fields.AnyAsync())
        {
            var fields = new List<Field>
            {
                new Field { FieldName = "North Field", AreaHectares = 12.5m, SoilType = "Loamy",     Location = "North Block A", CreatedAt = DateTime.Now },
                new Field { FieldName = "South Field", AreaHectares = 8.0m,  SoilType = "Clay",      Location = "South Block B", CreatedAt = DateTime.Now },
                new Field { FieldName = "East Field",  AreaHectares = 15.0m, SoilType = "Sandy Loam", Location = "East Block C",  CreatedAt = DateTime.Now }
            };

            await context.Fields.AddRangeAsync(fields);
            await context.SaveChangesAsync();
        }

        if (!await context.Crops.AnyAsync())
        {
            var fields = await context.Fields.ToListAsync();

            if (fields.Count >= 3)
            {
                var crops = new List<Crop>
                {
                    new Crop { CropName = "Wheat",  CropType = "Grain",     Season = SeasonType.Winter,  PlantingDate = new DateTime(2025, 11, 1), ExpectedHarvestDate = new DateTime(2026, 4, 15), FieldId = fields[0].FieldId, Status = "Growing" },
                    new Crop { CropName = "Rice",   CropType = "Grain",     Season = SeasonType.Monsoon, PlantingDate = new DateTime(2025, 6, 1),  ExpectedHarvestDate = new DateTime(2025, 10, 1), FieldId = fields[1].FieldId, Status = "Growing" },
                    new Crop { CropName = "Tomato", CropType = "Vegetable", Season = SeasonType.Summer,  PlantingDate = new DateTime(2026, 3, 1),  ExpectedHarvestDate = new DateTime(2026, 7, 1),  FieldId = fields[2].FieldId, Status = "Harvested" }
                };

                await context.Crops.AddRangeAsync(crops);
                await context.SaveChangesAsync();
            }
        }

        if (!await context.PlantingSchedules.AnyAsync())
        {
            var crops = await context.Crops.ToListAsync();
            var fields = await context.Fields.ToListAsync();

            if (crops.Count >= 3 && fields.Count >= 3)
            {
                var schedules = new List<PlantingSchedule>
                {
                    new PlantingSchedule { CropId = crops[0].CropId, FieldId = fields[0].FieldId, ScheduledDate = new DateTime(2026, 4, 15), ExpectedYieldKg = 4500.00m, Status = "Scheduled",  Notes = "First wheat harvest of the Winter season" },
                    new PlantingSchedule { CropId = crops[1].CropId, FieldId = fields[1].FieldId, ScheduledDate = new DateTime(2025, 10, 1), ExpectedYieldKg = 3800.00m, Status = "Scheduled",  Notes = "Main Monsoon rice harvest" },
                    new PlantingSchedule { CropId = crops[2].CropId, FieldId = fields[2].FieldId, ScheduledDate = new DateTime(2026, 7, 1),  ExpectedYieldKg = 3000.00m, Status = "Completed", Notes = "Summer tomato harvest completed" }
                };

                await context.PlantingSchedules.AddRangeAsync(schedules);
                await context.SaveChangesAsync();
            }
        }

        if (!await context.Harvests.AnyAsync())
        {
            var completedSchedule = await context.PlantingSchedules
                .FirstOrDefaultAsync(ps => ps.Status == "Completed");

            if (completedSchedule != null)
            {
                context.Harvests.Add(new Harvest
                {
                    ScheduleId = completedSchedule.ScheduleId,
                    HarvestedDate = new DateTime(2026, 7, 1),
                    ActualYieldKg = 3200.00m,
                    Notes = "Good yield despite dry spell"
                });
                await context.SaveChangesAsync();
            }
        }

        if (!await context.PestIncidents.AnyAsync())
        {
            var crops = await context.Crops.ToListAsync();

            var incidents = new List<PestIncident>
            {
                new PestIncident { CropId = crops[0].CropId, PestName = "Aphids",            Description = "Heavy aphid infestation on wheat leaves",    ReportedDate = DateTime.Now.AddDays(-5),  Status = IncidentStatus.Active,     DiseaseName = null },
                new PestIncident { CropId = crops[1].CropId, PestName = "Brown Plant Hopper", Description = "Early signs of BPH detected in rice field",   ReportedDate = DateTime.Now.AddDays(-10), Status = IncidentStatus.Monitoring, DiseaseName = "Hopper Burn" },
                new PestIncident { CropId = crops[2].CropId, PestName = "Whitefly",           Description = "Whitefly attack resolved after treatment",    ReportedDate = DateTime.Now.AddDays(-20), Status = IncidentStatus.Resolved,   DiseaseName = "Leaf Curl" }
            };

            await context.PestIncidents.AddRangeAsync(incidents);
            await context.SaveChangesAsync();
        }

        if (!await context.Resources.AnyAsync())
        {
            var resources = new List<Resource>
            {
                new Resource { Name = "NPK Fertilizer",   Type = ResourceType.Fertilizer, Quantity = 500.00m, Unit = "kg",          LastUpdated = DateTime.Now },
                new Resource { Name = "Chlorpyrifos",     Type = ResourceType.Pesticide,  Quantity = 8.00m,   Unit = "liters",      LastUpdated = DateTime.Now },
                new Resource { Name = "Wheat Seeds",      Type = ResourceType.Seeds,      Quantity = 200.00m, Unit = "kg",          LastUpdated = DateTime.Now },
                new Resource { Name = "Tractor",          Type = ResourceType.Equipment,  Quantity = 2.00m,   Unit = "units",       LastUpdated = DateTime.Now },
                new Resource { Name = "Irrigation Water", Type = ResourceType.Water,      Quantity = 5.00m,   Unit = "kiloliters",  LastUpdated = DateTime.Now }
            };

            await context.Resources.AddRangeAsync(resources);
            await context.SaveChangesAsync();
        }

        if (!await context.ResourceUsages.AnyAsync())
        {
            var schedule = await context.PlantingSchedules.FirstOrDefaultAsync();
            var resource = await context.Resources.FirstOrDefaultAsync();

            if (schedule != null && resource != null)
            {
                context.ResourceUsages.Add(new ResourceUsage
                {
                    ResourceId   = resource.ResourceId,
                    ScheduleId   = schedule.ScheduleId,
                    QuantityUsed = 50.00m,
                    UsedDate     = DateTime.Now.AddDays(-3),
                    Notes        = "Applied during pre-planting preparation"
                });
                await context.SaveChangesAsync();
            }
        }

        if (!await context.YieldReports.AnyAsync())
        {
            var crops = await context.Crops.ToListAsync();

            if (crops.Count >= 3)
            {
                var yieldReports = new List<YieldReport>
                {
                    new YieldReport { CropId = crops[0].CropId, TotalYieldKg = 4500.00m, Season = SeasonType.Winter,  Year = 2026, GeneratedAt = DateTime.Now },
                    new YieldReport { CropId = crops[1].CropId, TotalYieldKg = 3800.00m, Season = SeasonType.Monsoon, Year = 2025, GeneratedAt = DateTime.Now },
                    new YieldReport { CropId = crops[2].CropId, TotalYieldKg = 3200.00m, Season = SeasonType.Summer,  Year = 2026, GeneratedAt = DateTime.Now }
                };

                await context.YieldReports.AddRangeAsync(yieldReports);
                await context.SaveChangesAsync();
            }
        }
    }
}
