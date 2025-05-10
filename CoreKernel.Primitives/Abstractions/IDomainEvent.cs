namespace CoreKernel.Primitives.Abstractions;

/// <summary>
/// Interface to represent a Domain Event.
/// A domain event is a significant occurrence or change in state within the domain.
/// </summary>
public interface IDomainEvent
{
    // This interface is intentionally left blank and serves as a marker for domain events.
    // It is extended by CoreKernel.Messaging.Events.IDomainEvent for use in the messaging context.
    // Use CoreKernel.Messaging.Events.IDomainEvent for implementing domain events.
}