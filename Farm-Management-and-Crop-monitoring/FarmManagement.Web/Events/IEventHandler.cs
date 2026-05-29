namespace FarmManagement.Web.Events;

// Observer Pattern — contract every event handler must implement
public interface IEventHandler<T> where T : IDomainEvent
{
    Task HandleAsync(T domainEvent);
}
