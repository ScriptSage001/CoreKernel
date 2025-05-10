namespace CoreKernel.Messaging.Events;

/// <summary>
/// Marker interface for domain event handlers.
/// Defines a domain event handler for a specific domain event.
/// </summary>
/// <typeparam name="TEvent">
/// The type of the domain event being handled. Must implement the <see cref="IDomainEvent"/> interface.
/// </typeparam>
public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent;