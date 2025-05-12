using System;
using System.Collections.Generic;

namespace CoreKernel.Primitives.ValueObjects;

/// <summary>
/// Represents a strongly-typed ID value object.
/// This abstract class enforces type safety and ensures that IDs of different types cannot be compared.
/// </summary>
/// <typeparam name="T">The type of the underlying value of the ID. Must be non-nullable.</typeparam>
public abstract class StronglyTypedId<T> : IEquatable<StronglyTypedId<T>>
    where T : notnull
{
    /// <summary>
    /// Gets the underlying value of the strongly-typed ID.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StronglyTypedId{T}"/> class.
    /// </summary>
    /// <param name="value">The underlying value of the ID. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided value is null.</exception>
    protected StronglyTypedId(T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value), "ID value cannot be null");

        Value = value;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is StronglyTypedId<T> other && Equals(other);

    /// <summary>
    /// Determines whether the specified <see cref="StronglyTypedId{T}"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The other strongly-typed ID to compare with.</param>
    /// <returns><c>true</c> if the IDs are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(StronglyTypedId<T>? other) =>
        other is not null && EqualityComparer<T>.Default.Equals(Value, other.Value) &&
        GetType() == other.GetType(); // prevents collisions between different derived types

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(Value, GetType());

    /// <inheritdoc />
    public override string ToString() => Value.ToString() ?? string.Empty;

    /// <summary>
    /// Determines whether two <see cref="StronglyTypedId{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(StronglyTypedId<T>? left, StronglyTypedId<T>? right) =>
        EqualityComparer<StronglyTypedId<T>>.Default.Equals(left, right);

    /// <summary>
    /// Determines whether two <see cref="StronglyTypedId{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(StronglyTypedId<T>? left, StronglyTypedId<T>? right) =>
        !(left == right);
}