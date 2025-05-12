using System;

namespace CoreKernel.Primitives.Entities;

/// <summary>
/// Base Entity Class for Domain Entities.
/// Provides a base implementation for entities with an identifier and equality checks.
/// </summary>
/// <typeparam name="TId">The type of the identifier for the entity. Must be non-nullable.</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier for the entity.</param>
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}"/> class.
    /// This parameterless constructor is intended for use by derived classes.
    /// </summary>
    protected Entity()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public TId Id { get; private init; }

    #endregion

    #region Equatable Functions

    /// <summary>
    /// Determines whether the specified <see cref="Entity{TId}"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The other entity to compare with.</param>
    /// <returns><c>true</c> if the entities are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(Entity<TId>? other)
    {
        if (other is null ||
            other.GetType() != GetType())
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the object is equal to the current instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj.GetType() != GetType() ||
            obj is not Entity<TId> entity)
        {
            return false;
        }

        return entity.Id.Equals(Id);
    }

    /// <summary>
    /// Determines whether two <see cref="Entity{TId}"/> instances are equal.
    /// </summary>
    /// <param name="first">The first entity to compare.</param>
    /// <param name="second">The second entity to compare.</param>
    /// <returns><c>true</c> if the entities are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Entity<TId>? first, Entity<TId>? second)
    {
        return first is not null && second is not null && first.Equals(second);
    }

    /// <summary>
    /// Determines whether two <see cref="Entity{TId}"/> instances are not equal.
    /// </summary>
    /// <param name="first">The first entity to compare.</param>
    /// <param name="second">The second entity to compare.</param>
    /// <returns><c>true</c> if the entities are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Entity<TId>? first, Entity<TId>? second)
    {
        return !(first == second);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    #endregion
}