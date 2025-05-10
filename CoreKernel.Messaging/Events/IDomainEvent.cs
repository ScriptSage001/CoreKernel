namespace CoreKernel.Messaging.Events;

/// <summary>
/// Interface to represent a Domain Event.
/// A domain event is a significant occurrence or change in state within the domain.
/// </summary>
public interface IDomainEvent : Primitives.Abstractions.IDomainEvent, IEvent;