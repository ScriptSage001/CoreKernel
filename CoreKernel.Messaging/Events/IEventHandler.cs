using MediatR;

namespace CoreKernel.Messaging.Events;

/// <summary>
/// Marker interface for an event handler.
/// Defines an event handler for a specific event type.
/// </summary>
/// <typeparam name="TEvent">
/// The type of the event being handled. Must implement the <see cref="IEvent"/> interface.
/// </typeparam>
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent;