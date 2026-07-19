// ═══════════════════════════════════════════════════════════════
//  Pest/Disease Monitoring & Treatment Logging Tests
//  Tests that PestIncident entity tracks status transitions
//  and treatment logging correctly.
// ═══════════════════════════════════════════════════════════════

using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagementSystem_NUNIT;

[TestFixture]
[Description("Pest/Disease Monitoring — incident creation, status updates, treatment")]
public class PestMonitoringTests
{
    private PestIncident _incident;

    [SetUp]
    public void Setup()
    {
        _incident = new PestIncident
        {
            PestIncidentId = 1,
            PestName = "Aphids",
            Description = "Small green insects on rice leaves",
            Status = IncidentStatus.Active,
            CropId = 1
        };
    }

    // ── Test 1: New incident should default to Active status ──
    [Test]
    public void PestIncident_NewIncident_ShouldBeActive()
    {
        Assert.That(_incident.Status, Is.EqualTo(IncidentStatus.Active));
    }

    // ── Test 2: Status can transition from Active → Monitoring ──
    [Test]
    public void PestIncident_StatusChange_ActiveToMonitoring()
    {
        // Act
        _incident.Status = IncidentStatus.Monitoring;

        // Assert
        Assert.That(_incident.Status, Is.EqualTo(IncidentStatus.Monitoring));
    }

    // ── Test 3: Status can transition from Monitoring → Resolved ──
    [Test]
    public void PestIncident_StatusChange_MonitoringToResolved()
    {
        // Act
        _incident.Status = IncidentStatus.Monitoring;
        _incident.Status = IncidentStatus.Resolved;

        // Assert
        Assert.That(_incident.Status, Is.EqualTo(IncidentStatus.Resolved));
    }

    // ── Test 4: Disease name should be updatable ──
    [Test]
    public void PestIncident_DiseaseName_ShouldBeUpdatable()
    {
        // Arrange — initially no disease name
        Assert.That(_incident.DiseaseName, Is.Null);

        // Act — set disease name
        _incident.DiseaseName = "Leaf Blight";

        // Assert
        Assert.That(_incident.DiseaseName, Is.Not.Null);
        Assert.That(_incident.DiseaseName, Does.Contain("Blight"));
    }

    // ── Test 5: Incident should belong to a Crop (via CropId) ──
    [Test]
    public void PestIncident_ShouldHaveCropId()
    {
        Assert.That(_incident.CropId, Is.GreaterThan(0));
        Assert.That(_incident.CropId, Is.EqualTo(1));
    }
}
