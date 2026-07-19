using EnergyPulse.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Site> Sites { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<PowerReading> PowerReadings { get; set; }
    public DbSet<Alert> Alerts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.Site)
            .WithMany(s => s.Users)
            .HasForeignKey(u => u.SiteId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<MaintenanceRecord>()
            .HasOne(m => m.Device)
            .WithMany(d => d.MaintenanceRecords)
            .HasForeignKey(m => m.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MaintenanceRecord>()
            .HasOne(m => m.TechnicianAssigned)
            .WithMany(u => u.CreatedReports)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Alert>()
            .HasOne(a => a.Device)
            .WithMany(d => d.Alerts)
            .HasForeignKey(a => a.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Device>()
            .HasMany(d => d.Alerts)
            .WithOne(a => a.Device)
            .HasForeignKey(a => a.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}