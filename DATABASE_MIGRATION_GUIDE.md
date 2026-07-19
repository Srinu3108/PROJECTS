# EnergyPulse - Database Migration Guide

## Overview

This guide explains how to apply database migrations for the new User, MaintenanceRecord, and enhanced Device/Alert models.

## Prerequisites

- SQL Server installed and running
- EF Core Tools installed
- PowerShell or terminal access

## Step 1: Install EF Core Tools (if not already installed)

```bash
dotnet tool install --global dotnet-ef
```

## Step 2: Create Migration

Navigate to the EnergyPulse project directory and run:

```bash
cd c:\Users\kanup\source\repos\PROJECTS\EnergyPulse\EnergyPulse

# Create new migration
dotnet ef migrations add EnhancedModels
```

This creates a new migration file in the `Migrations` folder.

## Step 3: Apply Migration to Database

```bash
# Update database with the new migration
dotnet ef database update
```

## Migration Details

The migration will:

### New Tables Created:

1. **Users**
   - Id (Primary Key)
   - Username
   - Email
   - PasswordHash
   - Role (Admin/Technician)
   - SiteId (Foreign Key)
   - IsActive
   - CreatedAt
   - LastLogin

2. **MaintenanceRecords**
   - Id (Primary Key)
   - DeviceId (Foreign Key)
   - UserId (Foreign Key, nullable)
   - AlertId (Foreign Key, nullable)
   - Title
   - Description
   - ReportDate
   - CompletedDate
   - Status
   - Priority
   - EstimatedCost
   - ActualCost
   - Notes

### Existing Tables Modified:

1. **Devices**
   - Added: DeviceType (varchar)
   - Added: Capacity (float)
   - Added: InstalledDate (datetime)
   - Added: LastMaintenanceDate (datetime, nullable)
   - Added: CurrentEfficiency (float)
   - Added: LastReadingDate (datetime)

2. **Alerts**
   - Added: AlertType (varchar)
   - Added: Status (varchar)
   - Modified: CreatedAt (renamed from DateTime)
   - Added: ResolvedAt (datetime, nullable)
   - Added: ImpactKW (float, nullable)
   - Added: EstimatedCost (float, nullable)

3. **Sites**
   - Added: Type (varchar, default 'Solar')
   - Added: EstablishedDate (datetime)
   - Added: PerformanceTarget (float, default 85.0)
   - Added: CreatedAt (datetime)

### New Foreign Keys:

- Users.SiteId → Sites.Id
- MaintenanceRecords.DeviceId → Devices.Id
- MaintenanceRecords.UserId → Users.Id
- MaintenanceRecords.AlertId → Alerts.Id

## Rollback (If Needed)

To revert to previous database state:

```bash
# Remove the last migration (development only)
dotnet ef migrations remove

# Or revert the database to a previous migration
dotnet ef database update PreviousMigrationName
```

## Seed Data

The `Program.cs` has been updated with new seed data that includes:

- Enhanced Site with type and performance target
- Devices with capacity and installation dates
- Maintenance dates
- More detailed alerts with impact metrics

This seed data runs automatically on first run if the database is empty.

## Verification

After migration, verify the new tables exist:

```sql
-- Check new tables
SELECT * FROM Users;
SELECT * FROM MaintenanceRecords;

-- Check enhanced columns
SELECT
    Id, Name, Status, DeviceType, Capacity,
    CurrentEfficiency, InstalledDate, LastMaintenanceDate
FROM Devices;

SELECT
    Id, DeviceId, AlertType, Message, Status,
    Severity, ImpactKW, EstimatedCost
FROM Alerts;

SELECT
    Id, Name, Type, TotalCapacityKW,
    PerformanceTarget, EstablishedDate
FROM Sites;
```

## Troubleshooting

### Issue: "Cannot find the specified migration"

**Solution:**

```bash
# Check available migrations
dotnet ef migrations list

# Verify Migrations folder exists and has new file
dir Migrations
```

### Issue: "The connection string is invalid"

**Solution:**
Update connection string in `Program.cs`:

```csharp
options.UseSqlServer("Server=YOUR_SERVER;Database=EnergyPulseDb;Trusted_Connection=True;TrustServerCertificate=True")
```

### Issue: "Cannot add a NOT NULL column to existing table with rows"

**Solution:**

1. Drop database: `dotnet ef database drop --force`
2. Recreate: `dotnet ef database update`
3. Or add default values in migration

### Issue: "Foreign key constraint fails"

**Solution:**

- Ensure seed data creates Sites first before Devices
- Check `AppDbContext.OnModelCreating()` for correct relationships
- Verify foreign key cascading rules are configured

## Additional Database Tasks

### Create Indexes for Performance

```sql
-- For performance queries
CREATE INDEX IX_PowerReadings_DeviceId_Timestamp
ON PowerReadings(DeviceId, Timestamp DESC);

CREATE INDEX IX_Alerts_DeviceId_Status
ON Alerts(DeviceId, Status);

CREATE INDEX IX_Devices_SiteId
ON Devices(SiteId);

-- For user lookups
CREATE INDEX IX_Users_Email
ON Users(Email);
```

### Backup Database

```bash
# Backup before migration
SQLCMD -S SREENU\SQLEXPRESS -E -Q "BACKUP DATABASE [EnergyPulseDb] TO DISK = 'C:\Backups\EnergyPulseDb_Backup.bak'"
```

### Monitor Migration Progress

```csharp
// In Program.cs, add logging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Database migration completed successfully");
```

## Next Steps

After successful migration:

1. Test API endpoints with new data
2. Generate test reports
3. Create test maintenance records
4. Verify alert generation
5. Deploy UI enhancements
6. Load test the application

## FAQ

**Q: Will migration delete existing data?**
A: No, migrations add/modify columns but preserve existing data. Seed data only runs on empty databases.

**Q: How often should I backup?**
A: Before each major deployment or migration.

**Q: Can I have multiple migrations pending?**
A: Yes, they execute in sequence. Each must be named and timestamped.

**Q: How do I see what SQL is generated?**
A: Check the migration file in `Migrations` folder for the `Up()` method.

For more information, visit: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/
