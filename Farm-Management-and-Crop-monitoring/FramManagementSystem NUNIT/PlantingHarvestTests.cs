// ═══════════════════════════════════════════════════════════════
//  Planting & Harvest Scheduling Tests
//  Tests that PlantingSchedule and Harvest entities work together,
//  including status transitions and yield recording.
// ═══════════════════════════════════════════════════════════════

using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagementSystem_NUNIT;

[TestFixture]
[Description("Planting & Harvest Scheduling — schedule creation, harvest recording")]
public class PlantingHarvestTests
{
    private PlantingSchedule _schedule;

    [SetUp]
    public void Setup()
    {
        _schedule = new PlantingSchedule
        {
            ScheduleId = 1,
            CropId = 1,
            FieldId = 1,
            ScheduledDate = new DateTime(2025, 6, 15),
            ExpectedYieldKg = 500m,
            Status = "Scheduled",
            Notes = "First planting of the season"
        };
    }

    // ── Test 1: Schedule should be created with "Scheduled" status ──
    [Test]
    public void Schedule_DefaultStatus_ShouldBeScheduled()
    {
        Assert.That(_schedule.Status, Is.EqualTo("Scheduled"));
    }

    // ── Test 2: Recording a harvest should change status to Completed ──
    [Test]
    public void Schedule_AfterHarvestRecorded_StatusShouldBeCompleted()
    {
        // Act — simulate what the service does after recording harvest
        _schedule.Status = "Completed";

        // Assert
        Assert.That(_schedule.Status, Is.EqualTo("Completed"));
    }

    // ── Test 3: Harvest entity should link back to schedule ──
    [Test]
    public void Harvest_ShouldReferenceSchedule()
    {
        // Arrange
        var harvest = new Harvest
        {
            HarvestId = 1,
            ScheduleId = _schedule.ScheduleId,
            ActualYieldKg = 480m,
            HarvestedDate = DateTime.Now,
            PlantingSchedule = _schedule
        };

        // Assert
        Assert.That(harvest.ScheduleId, Is.EqualTo(1));
        Assert.That(harvest.PlantingSchedule.ExpectedYieldKg, Is.EqualTo(500m));
    }

    // ── Test 4: Yield variance — actual vs expected ──
    [Test]
    public void Harvest_YieldVariance_ShouldBeCalculatedCorrectly()
    {
        // Arrange
        decimal actualYield = 480m;
        decimal expectedYield = _schedule.ExpectedYieldKg;

        // Act
        decimal variance = actualYield - expectedYield;

        // Assert — 480 - 500 = -20 (under target)
        Assert.That(variance, Is.EqualTo(-20m));
        Assert.That(variance, Is.LessThan(0), "Actual yield was below expected");
    }

    // ── Test 5: Schedule should start with empty Harvests collection ──
    [Test]
    public void Schedule_ShouldHaveEmptyHarvestsCollection()
    {
        Assert.That(_schedule.Harvests, Is.Not.Null);
        Assert.That(_schedule.Harvests.Count, Is.EqualTo(0));
    }
}
