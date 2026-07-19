using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.States;

public class MonitoringPestState : IPestIncidentState
{
    public string StatusName => "Monitoring";

    public PestIncident Transition(PestIncident incident, string targetStatus)
    {
        switch (targetStatus)
        {
            case "Resolved":
                incident.Status = IncidentStatus.Resolved;
                return incident;
            case "Active":
                incident.Status = IncidentStatus.Active;
                return incident;
            default:
                throw new InvalidOperationException(
                    $"Cannot transition from Monitoring to '{targetStatus}'. Allowed: 'Active', 'Resolved'.");
        }
    }
}
