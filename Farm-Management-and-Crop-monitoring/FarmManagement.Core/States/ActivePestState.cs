using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.States;

public class ActivePestState : IPestIncidentState
{
    public string StatusName => "Active";

    public PestIncident Transition(PestIncident incident, string targetStatus)
    {
        if (targetStatus == "Monitoring")
        {
            incident.Status = IncidentStatus.Monitoring;
            return incident;
        }
        throw new InvalidOperationException(
            $"Cannot transition from Active to '{targetStatus}'. Only 'Monitoring' is allowed next.");
    }
}
