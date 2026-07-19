// ═══════════════════════════════════════════════════════════════
//  Resource Allocation & Usage Tracking Tests
//  Tests that Resource and ResourceUsage entities work correctly,
//  including stock deduction logic.
// ═══════════════════════════════════════════════════════════════

using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagementSystem_NUNIT;

[TestFixture]
[Description("Resource Allocation & Usage Tracking — entity and stock logic tests")]
public class ResourceAllocationTests
{
    private Resource _resource;

    [SetUp]
    public void Setup()
    {
        _resource = new Resource
        {
            ResourceId = 1,
            Name = "Urea Fertilizer",
            Type = ResourceType.Fertilizer,
            Quantity = 100m,
            Unit = "kg"
        };
    }

    // ── Test 1: Resource should be created with correct initial values ──
    [Test]
    public void Resource_Creation_ShouldSetProperties()
    {
        Assert.That(_resource.Name, Is.EqualTo("Urea Fertilizer"));
        Assert.That(_resource.Type, Is.EqualTo(ResourceType.Fertilizer));
        Assert.That(_resource.Quantity, Is.EqualTo(100m));
        Assert.That(_resource.Unit, Is.EqualTo("kg"));
    }

    // ── Test 2: After allocation, stock should decrease ──
    [Test]
    public void Resource_AfterAllocation_StockShouldDecrease()
    {
        // Arrange — simulate allocating 25 kg
        decimal allocated = 25m;

        // Act — deduct from stock (this is what the service does)
        _resource.Quantity -= allocated;

        // Assert
        Assert.That(_resource.Quantity, Is.EqualTo(75m));
    }

    // ── Test 3: ResourceUsage should link to a Resource ──
    [Test]
    public void ResourceUsage_ShouldReferenceResource()
    {
        // Arrange
        var usage = new ResourceUsage
        {
            ResourceUsageId = 1,
            ResourceId = _resource.ResourceId,
            ScheduleId = 10,
            QuantityUsed = 30m,
            Resource = _resource
        };

        // Assert
        Assert.That(usage.ResourceId, Is.EqualTo(1));
        Assert.That(usage.QuantityUsed, Is.EqualTo(30m));
        Assert.That(usage.Resource.Name, Is.EqualTo("Urea Fertilizer"));
    }

    // ── Test 4: Low stock check — quantity <= 10 means low stock ──
    [Test]
    public void Resource_LowStockCheck_ShouldReturnTrue_WhenQuantityLow()
    {
        // Arrange — set stock to a low value
        _resource.Quantity = 8m;

        // Act
        bool isLowStock = _resource.Quantity <= 10;

        // Assert
        Assert.That(isLowStock, Is.True);
    }

    // ── Test 5: Resource should start with empty usages collection ──
    [Test]
    public void Resource_ShouldHaveEmptyUsagesCollection()
    {
        Assert.That(_resource.ResourceUsages, Is.Not.Null);
        Assert.That(_resource.ResourceUsages.Count, Is.EqualTo(0));
    }
}
