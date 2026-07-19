namespace FarmManagement.Web.Events;

// Observer Pattern — dispatches events to all registered handlers
public interface IEventDispatcher
{
    Task DispatchAsync<T>(T domainEvent) where T : IDomainEvent;
}
