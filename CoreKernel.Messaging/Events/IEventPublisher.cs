using System.Threading.Tasks;

namespace CoreKernel.Messaging.Events;

/// <summary>
/// Defines a contract for publishing events.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to publish.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
}