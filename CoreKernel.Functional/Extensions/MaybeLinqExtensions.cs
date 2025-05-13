using System;
using System.Collections.Generic;
using System.Linq;
using CoreKernel.Functional.Maybe;

namespace CoreKernel.Functional.Extensions;

/// <summary>
/// Provides LINQ-like extension methods for working with <see cref="Maybe{T}"/> instances.
/// </summary>
public static class MaybeLinqExtensions
{
    /// <summary>
    /// Projects the value of a <see cref="Maybe{T}"/> into a new form if a value is present.
    /// </summary>
    /// <typeparam name="TSource">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <param name="selector">The transformation function to apply to the value.</param>
    /// <returns>A new Maybe containing the transformed value, or None if the source contains no value.</returns>
    public static Maybe<TResult> Select<TSource, TResult>(
        this Maybe<TSource> source,
        Func<TSource, TResult> selector)
    {
        return source.Map(selector);
    }

    /// <summary>
    /// Projects the value of a <see cref="Maybe{T}"/> into a new <see cref="Maybe{TResult}"/>
    /// and flattens the result.
    /// </summary>
    /// <typeparam name="TSource">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <param name="selector">The transformation function to apply to the value.</param>
    /// <returns>The result of the selector if the source contains a value, or None if the source contains no value.</returns>
    public static Maybe<TResult> SelectMany<TSource, TResult>(
        this Maybe<TSource> source,
        Func<TSource, Maybe<TResult>> selector)
    {
        return source.Bind(selector);
    }

    /// <summary>
    /// Projects the value of a <see cref="Maybe{T}"/> into a new <see cref="Maybe{TResult}"/>
    /// and flattens the result, incorporating the original value using a result selector.
    /// </summary>
    /// <typeparam name="TSource">The type of the source value.</typeparam>
    /// <typeparam name="TIntermediate">The type of the intermediate value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <param name="collectionSelector">The transformation function to apply to the value.</param>
    /// <param name="resultSelector">The function that combines the original value and the intermediate value.</param>
    /// <returns>A new Maybe containing the transformed value, or None if the source contains no value or the intermediate contains no value.</returns>
    public static Maybe<TResult> SelectMany<TSource, TIntermediate, TResult>(
        this Maybe<TSource> source,
        Func<TSource, Maybe<TIntermediate>> collectionSelector,
        Func<TSource, TIntermediate, TResult> resultSelector)
    {
        return source.Bind(
            sourceValue => collectionSelector(sourceValue)
                .Map(intermediateValue => resultSelector(sourceValue, intermediateValue)));
    }

    /// <summary>
    /// Filters a <see cref="Maybe{T}"/> based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <param name="predicate">The predicate to apply to the value if present.</param>
    /// <returns>The source Maybe if it contains a value that satisfies the predicate, otherwise None.</returns>
    public static Maybe<T> Where<T>(this Maybe<T> source, Func<T, bool> predicate)
    {
        return source.Bind(value => predicate(value) ? source : Maybe<T>.None);
    }

    /// <summary>
    /// Determines whether a <see cref="Maybe{T}"/> contains a value that satisfies a condition.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <param name="predicate">The predicate to apply to the value if present.</param>
    /// <returns><c>true</c> if the Maybe contains a value that satisfies the predicate, otherwise <c>false</c>.</returns>
    public static bool Any<T>(this Maybe<T> source, Func<T, bool> predicate)
    {
        return source.Match(predicate, () => false);
    }

    /// <summary>
    /// Determines whether a <see cref="Maybe{T}"/> contains a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <returns><c>true</c> if the Maybe contains a value, otherwise <c>false</c>.</returns>
    public static bool Any<T>(this Maybe<T> source)
    {
        return source.HasValue;
    }

    /// <summary>
    /// Determines whether a <see cref="Maybe{T}"/> contains a value that satisfies a condition.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <param name="predicate">The predicate to apply to the value if present.</param>
    /// <returns><c>true</c> if the Maybe contains a value that satisfies the predicate, otherwise <c>false</c>.</returns>
    public static bool All<T>(this Maybe<T> source, Func<T, bool> predicate)
    {
        return source.Match(predicate, () => true);
    }

    /// <summary>
    /// Returns the value of a <see cref="Maybe{T}"/> if it has a value, or the specified default value if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <param name="defaultValue">The default value to return if the Maybe contains no value.</param>
    /// <returns>The value of the Maybe if it has a value, or the default value if it does not.</returns>
    public static T FirstOrDefault<T>(this Maybe<T> source, T defaultValue = default)
    {
        return source.Match(value => value, () => defaultValue);
    }

    /// <summary>
    /// Returns the value of a <see cref="Maybe{T}"/> if it has a value, or throws an exception if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <returns>The value of the Maybe if it has a value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Maybe contains no value.</exception>
    public static T First<T>(this Maybe<T> source)
    {
        return source.ValueOrThrow();
    }

    /// <summary>
    /// Returns the single value of a <see cref="Maybe{T}"/> if it has a value, or throws an exception if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <returns>The value of the Maybe if it has a value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Maybe contains no value.</exception>
    public static T Single<T>(this Maybe<T> source)
    {
        return source.ValueOrThrow();
    }

    /// <summary>
    /// Returns the single value of a <see cref="Maybe{T}"/> if it has a value, or the specified default value if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="source">The source, Maybe.</param>
    /// <param name="defaultValue">The default value to return if the Maybe contains no value.</param>
    /// <returns>The value of the Maybe if it has a value, or the default value if it does not.</returns>
    public static T SingleOrDefault<T>(this Maybe<T> source, T defaultValue = default)
    {
        return source.Match(value => value, () => defaultValue);
    }

    /// <summary>
    /// Creates a new <see cref="Maybe{T}"/> containing the first element of the sequence that satisfies a condition,
    /// or None if no such element is found.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>A Maybe containing the first element that satisfies the condition, or None if no such element is found.</returns>
    public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
    {
        try
        {
            if (predicate == null)
            {
                var result = source.FirstOrDefault();
                return result is not null ? Maybe<T>.Some(result) : Maybe<T>.None;
            }
            else
            {
                var result = source.FirstOrDefault(predicate);
                return result is not null ? Maybe<T>.Some(result) : Maybe<T>.None;
            }
        }
        catch (InvalidOperationException)
        {
            return Maybe<T>.None;
        }
    }

    /// <summary>
    /// Creates a new <see cref="Maybe{T}"/> containing the single element of the sequence that satisfies a condition,
    /// or None if no such element is found or if more than one element satisfies the condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>A Maybe containing the single element that satisfies the condition, or None if no such element is found or if more than one element satisfies the condition.</returns>
    public static Maybe<T> SingleOrNone<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
    {
        try
        {
            if (predicate == null)
            {
                var result = source.SingleOrDefault();
                return result is not null ? Maybe<T>.Some(result) : Maybe<T>.None;
            }
            else
            {
                var result = source.SingleOrDefault(predicate);
                return result is not null ? Maybe<T>.Some(result) : Maybe<T>.None;
            }
        }
        catch (InvalidOperationException)
        {
            return Maybe<T>.None;
        }
    }
}