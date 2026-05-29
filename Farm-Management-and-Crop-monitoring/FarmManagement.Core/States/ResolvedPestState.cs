using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.States;

public class ResolvedPestState : IPestIncidentState
{
    public string StatusName => "Resolved";

    public PestIncident Transition(PestIncident incident, string targetStatus)
        => throw new InvalidOperationException(
            "Pest incident is already Resolved. No further status transitions are allowed.");
}
