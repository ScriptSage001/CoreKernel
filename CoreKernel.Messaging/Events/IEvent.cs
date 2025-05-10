using MediatR;

namespace CoreKernel.Messaging.Events;

/// <summary>
/// Marker interface to represent an event.
/// Implements the <see cref="INotification"/> interface from MediatR to enable event-based communication.
/// </summary>
public interface IEvent : INotification
{
    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the timestamp indicating when the event occurred.
    /// </summary>
    public DateTime TimeStamp { get; init; }

    /// <summary>
    /// Gets the correlation identifier used to trace the event across systems.
    /// </summary>
    public Guid CorrelationId { get; init; }
}