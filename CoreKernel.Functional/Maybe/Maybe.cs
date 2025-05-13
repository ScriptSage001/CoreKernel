using System;
using System.Collections.Generic;

namespace CoreKernel.Functional.Maybe;

/// <summary>
/// Represents an optional value that may or may not contain a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value that may be contained.</typeparam>
public readonly struct Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T _value;

    /// <summary>
    /// Gets a value indicating whether this instance contains a value.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Gets the value if present; otherwise, throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <returns>The value contained in this instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no value is present.</exception>
    public T ValueOrThrow()
    {
        if (!HasValue)
            throw new InvalidOperationException("No value present.");
        return _value!;
    }

    /// <summary>
    /// Gets the value if present; otherwise, returns the default value for the type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>The value or the default value.</returns>
    public T? ValueOrDefault() => HasValue ? _value : default;

    private Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    /// <summary>
    /// Creates a new <see cref="Maybe{T}"/> instance containing the specified value.
    /// </summary>
    /// <param name="value">The value to contain.</param>
    /// <returns>A <see cref="Maybe{T}"/> instance containing the value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
    public static Maybe<T> Some(T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value), "Cannot assign null to Maybe.Some");

        return new Maybe<T>(value);
    }

    /// <summary>
    /// Represents an empty <see cref="Maybe{T}"/> instance.
    /// </summary>
    public static readonly Maybe<T> None = new Maybe<T>();

    /// <summary>
    /// Matches the current instance to one of two functions based on whether it contains a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onSome">The function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
    {
        return HasValue ? onSome(_value!) : onNone();
    }

    /// <summary>
    /// Matches the current instance to one of two actions based on whether it contains a value.
    /// </summary>
    /// <param name="onSome">The action to execute if a value is present.</param>
    /// <param name="onNone">The action to execute if no value is present.</param>
    public void Match(Action<T> onSome, Action onNone)
    {
        if (HasValue) onSome(_value!);
        else onNone();
    }

    /// <summary>
    /// Transforms the value contained in this instance using the specified mapping function.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="mapper">The mapping function to apply.</param>
    /// <returns>A new <see cref="Maybe{TResult}"/> containing the transformed value, or <see cref="Maybe{TResult}.None"/> if no value is present.</returns>
    public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return HasValue ? Maybe<TResult>.Some(mapper(_value!)) : Maybe<TResult>.None;
    }

    /// <summary>
    /// Transforms the value contained in this instance using the specified binding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="binder">The binding function to apply.</param>
    /// <returns>A new <see cref="Maybe{TResult}"/> returned by the binding function, or <see cref="Maybe{TResult}.None"/> if no value is present.</returns>
    public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> binder)
    {
        return HasValue ? binder(_value!) : Maybe<TResult>.None;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Maybe<T> other && Equals(other);
    }

    /// <summary>
    /// Determines whether the current instance is equal to another <see cref="Maybe{T}"/> instance.
    /// </summary>
    /// <param name="other">The other instance to compare to.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(Maybe<T> other)
    {
        if (!HasValue && !other.HasValue)
            return true;
        if (HasValue != other.HasValue)
            return false;
        return EqualityComparer<T>.Default.Equals(_value!, other._value!);
    }
    
    /// <summary>
    /// Determines whether two <see cref="Maybe{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Maybe<T>? left, Maybe<T>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    
    /// <summary>
    /// Determines whether two <see cref="Maybe{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Maybe<T>? left, Maybe<T>? right)
    {
        return !(left == right);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HasValue ? _value!.GetHashCode() : 0;
    }

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a <see cref="Maybe{T}"/> instance.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Maybe<T>(T value)
        => value is null ? None : Some(value);

    /// <inheritdoc />
    public override string ToString()
        => HasValue ? $"Some({_value})" : "None";
}