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
}