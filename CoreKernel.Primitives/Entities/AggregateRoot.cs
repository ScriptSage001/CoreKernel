using System.Collections.Generic;
using CoreKernel.Primitives.Abstractions;

namespace CoreKernel.Primitives.Entities;

/// <summary>
/// Base Class for Aggregate Roots.
/// Represents an entity that serves as the root of an aggregate in the domain model.
/// </summary>
/// <typeparam name="TId">The type of the identifier for the aggregate root. Must be non-nullable.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
{
    #region Fields

    /// <summary>
    /// List of domain events that have occurred on this aggregate root.
    /// Used to track changes within the aggregate that need to be published as events.
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = [];

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot{TId}"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier for the aggregate root.</param>
    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Parameterless constructor for scenarios where the ID is autogenerated by the database.
    /// </summary>
    protected AggregateRoot()
    {
    }

    #endregion

    #region Domain Events

    /// <summary>
    /// Raises a domain event and adds it to the list of domain events.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    public void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Gets the list of domain events that have occurred on this aggregate root.
    /// </summary>
    /// <returns>A read-only collection of domain events.</returns>
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => [.. _domainEvents];

    /// <summary>
    /// Clears the list of domain events that have occurred on this aggregate root.
    /// This is typically called after the events have been published.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    #endregion
}