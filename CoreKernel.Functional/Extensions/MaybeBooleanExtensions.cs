using System;
using CoreKernel.Functional.Maybe;

namespace CoreKernel.Functional.Extensions;

/// <summary>
/// Provides extension methods for working with boolean values and <see cref="Maybe{T}"/> instances.
/// </summary>
public static class MaybeBooleanExtensions
{
    /// <summary>
    /// Executes an action if the boolean value is true.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <param name="action">The action to execute if the condition is true.</param>
    /// <returns>The original boolean value.</returns>
    public static bool Then(this bool condition, Action action)
    {
        if (condition)
        {
            action();
        }

        return condition;
    }

    /// <summary>
    /// Executes an action if the boolean value is false.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <param name="action">The action to execute if the condition is false.</param>
    /// <returns>The original boolean value.</returns>
    public static bool Else(this bool condition, Action action)
    {
        if (!condition)
        {
            action();
        }

        return condition;
    }

    /// <summary>
    /// Creates a <see cref="Maybe{T}"/> containing a value if the condition is true, or None if it is false.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="condition">The boolean condition to check.</param>
    /// <param name="value">The value to contain if the condition is true.</param>
    /// <returns>A Maybe containing the value if the condition is true, or None if it is false.</returns>
    public static Maybe<T> ToMaybe<T>(this bool condition, T value)
        => condition ? Maybe<T>.Some(value) : Maybe<T>.None;

    /// <summary>
    /// Creates a <see cref="Maybe{T}"/> using a factory function if the condition is true, or None if it is false.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="condition">The boolean condition to check.</param>
    /// <param name="valueFactory">The factory function to create the value if the condition is true.</param>
    /// <returns>A Maybe containing the value created by the factory if the condition is true, or None if it is false.</returns>
    public static Maybe<T> ToMaybe<T>(this bool condition, Func<T> valueFactory)
        => condition ? Maybe<T>.Some(valueFactory()) : Maybe<T>.None;

    /// <summary>
    /// Executes one of two actions based on the boolean value.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <param name="onTrue">The action to execute if the condition is true.</param>
    /// <param name="onFalse">The action to execute if the condition is false.</param>
    /// <returns>The original boolean value.</returns>
    public static bool Match(this bool condition, Action onTrue, Action onFalse)
    {
        if (condition)
        {
            onTrue();
        }
        else
        {
            onFalse();
        }

        return condition;
    }

    /// <summary>
    /// Returns one of two values based on the boolean value.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="condition">The boolean condition to check.</param>
    /// <param name="onTrue">The value to return if the condition is true.</param>
    /// <param name="onFalse">The value to return if the condition is false.</param>
    /// <returns>The value corresponding to the boolean value.</returns>
    public static T Match<T>(this bool condition, T onTrue, T onFalse)
        => condition ? onTrue : onFalse;

    /// <summary>
    /// Returns one of two values based on the boolean value using factory functions.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="condition">The boolean condition to check.</param>
    /// <param name="onTrue">The factory function to create the value if the condition is true.</param>
    /// <param name="onFalse">The factory function to create the value if the condition is false.</param>
    /// <returns>The value corresponding to the boolean value.</returns>
    public static T Match<T>(this bool condition, Func<T> onTrue, Func<T> onFalse)
        => condition ? onTrue() : onFalse();

    /// <summary>
    /// Executes an action if a <see cref="Maybe{bool}"/> contains true.
    /// </summary>
    /// <param name="maybe">The Maybe boolean to check.</param>
    /// <param name="action">The action to execute if the Maybe contains true.</param>
    /// <returns>The original Maybe boolean.</returns>
    public static Maybe<bool> DoWhenTrue(this Maybe<bool> maybe, Action action)
    {
        maybe.Match(
            value =>
            {
                if (value) action();
            },
            () => { });
        return maybe;
    }

    /// <summary>
    /// Executes an action if a <see cref="Maybe{bool}"/> contains false.
    /// </summary>
    /// <param name="maybe">The Maybe boolean to check.</param>
    /// <param name="action">The action to execute if the Maybe contains false.</param>
    /// <returns>The original Maybe boolean.</returns>
    public static Maybe<bool> DoWhenFalse(this Maybe<bool> maybe, Action action)
    {
        maybe.Match(
            value =>
            {
                if (!value) action();
            },
            () => { });
        return maybe;
    }

    /// <summary>
    /// Filters a <see cref="Maybe{T}"/> to be None if the value doesn't satisfy the predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="maybe">The Maybe to filter.</param>
    /// <param name="predicate">The predicate to apply to the value if present.</param>
    /// <returns>The original Maybe if it contains a value that satisfies the predicate, otherwise None.</returns>
    public static Maybe<T> Filter<T>(this Maybe<T> maybe, Func<T, bool> predicate)
        => maybe.Bind(value => predicate(value) ? maybe : Maybe<T>.None);
}