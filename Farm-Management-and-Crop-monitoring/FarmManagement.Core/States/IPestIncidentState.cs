using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.States;

public interface IPestIncidentState
{
    string StatusName { get; }
    PestIncident Transition(PestIncident incident, string targetStatus);
}
