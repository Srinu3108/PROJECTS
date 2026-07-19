using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Data;

public class FarmDbContext : DbContext
{
    public FarmDbContext(DbContextOptions<FarmDbContext> options) : base(options) { }

    public DbSet<Crop> Crops { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<ResourceUsage> ResourceUsages { get; set; }
    public DbSet<PlantingSchedule> PlantingSchedules { get; set; }
    public DbSet<Harvest> Harvests { get; set; }
    public DbSet<PestIncident> PestIncidents { get; set; }
    public DbSet<YieldReport> YieldReports { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Crop>(entity =>
        {
            entity.HasKey(c => c.CropId);
            entity.Property(c => c.CropName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.CropType).HasMaxLength(100);
            entity.Property(c => c.Season).HasMaxLength(50);
            entity.Property(c => c.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(f => f.FieldId);
            entity.Property(f => f.FieldName).IsRequired().HasMaxLength(100);
            entity.Property(f => f.SoilType).HasMaxLength(100);
            entity.Property(f => f.Location).HasMaxLength(200);
            entity.Property(f => f.AreaHectares).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<PlantingSchedule>(entity =>
        {
            entity.HasKey(ps => ps.ScheduleId);
            entity.Property(ps => ps.Status).HasMaxLength(50);
            entity.Property(ps => ps.Notes).HasMaxLength(500);

            entity.HasOne(ps => ps.Crop)
                  .WithMany(c => c.PlantingSchedules)
                  .HasForeignKey(ps => ps.CropId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ps => ps.Field)
                  .WithMany(f => f.PlantingSchedules)
                  .HasForeignKey(ps => ps.FieldId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Harvest>(entity =>
        {
            entity.HasKey(h => h.HarvestId);
            entity.Property(h => h.ActualYieldKg).HasColumnType("decimal(10,2)");
            entity.Property(h => h.Notes).HasMaxLength(500);

            entity.HasOne(h => h.PlantingSchedule)
                  .WithMany(ps => ps.Harvests)
                  .HasForeignKey(h => h.ScheduleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PestIncident>(entity =>
        {
            entity.HasKey(p => p.PestIncidentId);
            entity.Property(p => p.PestName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.DiseaseName).HasMaxLength(200);

            entity.HasOne(p => p.Crop)
                  .WithMany(c => c.PestIncidents)
                  .HasForeignKey(p => p.CropId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(r => r.ResourceId);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Unit).HasMaxLength(50);
            entity.Property(r => r.Quantity).HasColumnType("decimal(10,2)");

            entity.HasOne(r => r.PestIncident)
                  .WithMany(p => p.Resources)
                  .HasForeignKey(r => r.PestIncidentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ResourceUsage>(entity =>
        {
            entity.HasKey(ru => ru.ResourceUsageId);
            entity.Property(ru => ru.QuantityUsed).HasColumnType("decimal(10,2)");
            entity.Property(ru => ru.Notes).HasMaxLength(500);

            entity.HasOne(ru => ru.Resource)
                  .WithMany(r => r.ResourceUsages)
                  .HasForeignKey(ru => ru.ResourceId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ru => ru.PlantingSchedule)
                  .WithMany(ps => ps.ResourceUsages)
                  .HasForeignKey(ru => ru.ScheduleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<YieldReport>(entity =>
        {
            entity.HasKey(y => y.YieldReportId);
            entity.Property(y => y.TotalYieldKg).HasColumnType("decimal(10,2)");
            entity.Property(y => y.AverageYieldPerAcre).HasColumnType("decimal(10,2)");
            entity.Property(y => y.Remarks).HasMaxLength(255);

            entity.HasOne(y => y.Crop)
                  .WithMany(c => c.YieldReports)
                  .HasForeignKey(y => y.CropId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(50);
            entity.Property(u => u.PasswordHint).HasMaxLength(200).HasDefaultValue(string.Empty);
        });

        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(a => a.ActivityLogId);
            entity.Property(a => a.UserName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UserRole).HasMaxLength(50);
            entity.Property(a => a.Action).HasMaxLength(50);
            entity.Property(a => a.EntityType).HasMaxLength(50);
            entity.Property(a => a.Description).HasMaxLength(500);
        });
    }
}
