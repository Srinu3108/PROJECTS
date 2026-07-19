using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.States;

public static class PestStateMachine
{
    public static IPestIncidentState GetState(IncidentStatus status) => status switch
    {
        IncidentStatus.Active     => new ActivePestState(),
        IncidentStatus.Monitoring => new MonitoringPestState(),
        IncidentStatus.Resolved   => new ResolvedPestState(),
        _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unknown status: {status}")
    };

    public static PestIncident Transition(PestIncident incident, string targetStatus)
    {
        var currentState = GetState(incident.Status);
        return currentState.Transition(incident, targetStatus);
    }
}
