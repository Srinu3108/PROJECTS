// ═══════════════════════════════════════════════════════════════
//  Yield Analytics & Farm Reporting Tests
//  Tests that YieldReport entity stores calculated values
//  and that basic analytics computations are correct.
// ═══════════════════════════════════════════════════════════════

using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagementSystem_NUNIT;

[TestFixture]
[Description("Yield Analytics & Farm Reporting — report generation and calculations")]
public class YieldAnalyticsTests
{
    private List<Harvest> _harvests;

    [SetUp]
    public void Setup()
    {
        // Simulate 3 harvest records for the same crop
        var schedule = new PlantingSchedule
        {
            ScheduleId = 1,
            CropId = 1,
            FieldId = 1,
            ExpectedYieldKg = 500m,
            Status = "Completed"
        };

        _harvests =
        [
            new Harvest { HarvestId = 1, ScheduleId = 1, ActualYieldKg = 450m, PlantingSchedule = schedule },
            new Harvest { HarvestId = 2, ScheduleId = 1, ActualYieldKg = 520m, PlantingSchedule = schedule },
            new Harvest { HarvestId = 3, ScheduleId = 1, ActualYieldKg = 480m, PlantingSchedule = schedule }
        ];
    }

    // ── Test 1: Total yield should sum all harvests ──
    [Test]
    public void YieldAnalytics_TotalYield_ShouldSumAllHarvests()
    {
        // Act
        decimal totalYield = _harvests.Sum(h => h.ActualYieldKg);

        // Assert — 450 + 520 + 480 = 1450
        Assert.That(totalYield, Is.EqualTo(1450m));
    }

    // ── Test 2: Average yield should be calculated correctly ──
    [Test]
    public void YieldAnalytics_AverageYield_ShouldBeCorrect()
    {
        // Act
        decimal average = _harvests.Average(h => h.ActualYieldKg);

        // Assert — 1450 / 3 ≈ 483.33
        Assert.That(average, Is.InRange(483m, 484m));
    }

    // ── Test 3: YieldReport entity should store generated values ──
    [Test]
    public void YieldReport_ShouldStoreCalculatedValues()
    {
        // Arrange
        var report = new YieldReport
        {
            YieldReportId = 1,
            CropId = 1,
            TotalYieldKg = 1450m,
            AverageYieldPerAcre = 263.6m,
            Season = SeasonType.Monsoon,
            Year = 2025,
            Remarks = "Auto-generated from 3 harvest records"
        };

        // Assert
        Assert.That(report.TotalYieldKg, Is.EqualTo(1450m));
        Assert.That(report.AverageYieldPerAcre, Is.GreaterThan(0));
        Assert.That(report.Year, Is.EqualTo(2025));
        Assert.That(report.Remarks, Does.Contain("3 harvest"));
    }

    // ── Test 4: Average yield per acre calculation ──
    [Test]
    public void YieldReport_AverageYieldPerAcre_ShouldCalculateCorrectly()
    {
        // Arrange
        decimal totalYield = 1450m;
        decimal fieldAreaHectares = 5.5m;

        // Act — avgPerAcre = total / area
        decimal avgPerAcre = totalYield / fieldAreaHectares;

        // Assert — 1450 / 5.5 ≈ 263.6
        Assert.That(avgPerAcre, Is.InRange(263m, 264m));
    }

    // ── Test 5: Harvest count should match expected number ──
    [Test]
    public void YieldAnalytics_HarvestCount_ShouldMatchRecords()
    {
        Assert.That(_harvests.Count, Is.EqualTo(3));
    }
}
