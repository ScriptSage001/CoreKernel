namespace CoreKernel.Primitives.ValueObjects;

/// <summary>
/// Base class for Value Objects.
/// Provides equality comparison based on the values of its properties.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Gets the atomic values that define the value object.
    /// Derived classes must override this method to provide the values for equality comparison.
    /// </summary>
    /// <returns>An enumerable of atomic values.</returns>
    protected abstract IEnumerable<object> GetAtomicValues();

    /// <summary>
    /// Determines whether the current value object is equal to another value object.
    /// </summary>
    /// <param name="other">The other value object to compare with.</param>
    /// <returns><c>true</c> if the value objects are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(ValueObject? other)
    {
        return other is not null && ValuesAreEqual(other);
    }

    /// <summary>
    /// Determines whether the current value object is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the object is a value object and is equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && ValuesAreEqual(other);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current value object.</returns>
    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(17, (current, value) => current * 31 + (value?.GetHashCode() ?? 0));
    }

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="first">The first value object to compare.</param>
    /// <param name="second">The second value object to compare.</param>
    /// <returns><c>true</c> if the value objects are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(ValueObject? first, ValueObject? second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;
        return first.Equals(second);
    }

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="first">The first value object to compare.</param>
    /// <param name="second">The second value object to compare.</param>
    /// <returns><c>true</c> if the value objects are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ValueObject? first, ValueObject? second)
    {
        return !(first == second);
    }

    /// <summary>
    /// Compares the atomic values of the current value object with another value object.
    /// </summary>
    /// <param name="other">The other value object to compare with.</param>
    /// <returns><c>true</c> if the atomic values are equal; otherwise, <c>false</c>.</returns>
    private bool ValuesAreEqual(ValueObject other)
    {
        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }
}