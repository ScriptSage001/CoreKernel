using System;
using CoreKernel.Functional.Maybe;

namespace CoreKernel.Functional.Extensions;

/// <summary>
/// Provides extension methods for composing <see cref="Maybe{T}"/> instances.
/// </summary>
public static class MaybeCompositionExtensions
{
    /// <summary>
    /// Returns the current Maybe if it has a value, or the alternate Maybe if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="maybe">The source 'Maybe'.</param>
    /// <param name="alternate">The alternate Maybe to return if the source contains no value.</param>
    /// <returns>The source Maybe if it has a value, or the alternate Maybe if it does not.</returns>
    public static Maybe<T> Or<T>(this Maybe<T> maybe, Maybe<T> alternate)
        => maybe.Match(
            _ => maybe,
            () => alternate);

    /// <summary>
    /// Returns the current Maybe if it has a value, or a Maybe created from the specified value if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="maybe">The source 'Maybe'.</param>
    /// <param name="alternateValue">The alternate value to wrap in a Maybe if the source contains no value.</param>
    /// <returns>The source Maybe if it has a value, or a Maybe containing the alternate value if it does not.</returns>
    public static Maybe<T> Or<T>(this Maybe<T> maybe, T alternateValue)
        => maybe.Match(
            _ => maybe,
            () => Maybe<T>.Some(alternateValue));

    /// <summary>
    /// Returns the current Maybe if it has a value, or a Maybe created from the specified factory function if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="maybe">The source 'Maybe'.</param>
    /// <param name="alternateFactory">The factory function to create an alternate value if the source contains no value.</param>
    /// <returns>The source Maybe if it has a value, or a Maybe containing the result of the factory function if it does not.</returns>
    public static Maybe<T> Or<T>(this Maybe<T> maybe, Func<T> alternateFactory)
        => maybe.Match(
            _ => maybe,
            () => Maybe<T>.Some(alternateFactory()));

    /// <summary>
    /// Returns the current Maybe if it has a value, or a Maybe created from the specified factory function if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="maybe">The source 'Maybe'.</param>
    /// <param name="alternateFactory">The factory function to create an alternate Maybe if the source contains no value.</param>
    /// <returns>The source Maybe if it has a value, or the result of the factory function if it does not.</returns>
    public static Maybe<T> OrElse<T>(this Maybe<T> maybe, Func<Maybe<T>> alternateFactory)
        => maybe.Match(
            _ => maybe,
            alternateFactory);

    /// <summary>
    /// Collapses a nested <see cref="Maybe{Maybe{T}}"/> into a single <see cref="Maybe{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="maybeMaybe">The nested 'Maybe'.</param>
    /// <returns>A flattened 'Maybe'.</returns>
    public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> maybeMaybe)
        => maybeMaybe.Match(
            innerMaybe => innerMaybe,
            () => Maybe<T>.None);

    /// <summary>
    /// Combines two Maybes into a single Maybe containing a tuple of their values.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="first">The first 'Maybe'.</param>
    /// <param name="second">The second 'Maybe'.</param>
    /// <returns>A Maybe containing a tuple of both values if both have values, or None if either has no value.</returns>
    public static Maybe<(T1, T2)> Zip<T1, T2>(this Maybe<T1> first, Maybe<T2> second)
        => first.Bind(firstValue =>
            second.Map(secondValue => (firstValue, secondValue)));

    /// <summary>
    /// Combines three Maybes into a single Maybe containing a tuple of their values.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="first">The first 'Maybe'.</param>
    /// <param name="second">The second 'Maybe'.</param>
    /// <param name="third">The third 'Maybe'.</param>
    /// <returns>A Maybe containing a tuple of all values if all have values, or None if any has no value.</returns>
    public static Maybe<(T1, T2, T3)> Zip<T1, T2, T3>(this Maybe<T1> first, Maybe<T2> second, Maybe<T3> third)
        => first.Bind(firstValue =>
            second.Bind(secondValue =>
                third.Map(thirdValue => (firstValue, secondValue, thirdValue))));

    /// <summary>
    /// Applies a function to the values of two Maybes.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="first">The first 'Maybe'.</param>
    /// <param name="second">The second 'Maybe'.</param>
    /// <param name="selector">The function to apply to both values.</param>
    /// <returns>A Maybe containing the result of the function if both Maybes have values, or None if either has no value.</returns>
    public static Maybe<TResult> Apply<T1, T2, TResult>(
        this Maybe<T1> first,
        Maybe<T2> second,
        Func<T1, T2, TResult> selector)
        => first.Bind(firstValue =>
            second.Map(secondValue => selector(firstValue, secondValue)));

    /// <summary>
    /// Applies a function to the values of three Maybes.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="first">The first 'Maybe'.</param>
    /// <param name="second">The second 'Maybe'.</param>
    /// <param name="third">The third 'Maybe'.</param>
    /// <param name="selector">The function to apply to all values.</param>
    /// <returns>A Maybe containing the result of the function if all Maybes have values, or None if any has no value.</returns>
    public static Maybe<TResult> Apply<T1, T2, T3, TResult>(
        this Maybe<T1> first,
        Maybe<T2> second,
        Maybe<T3> third,
        Func<T1, T2, T3, TResult> selector)
        => first.Bind(firstValue =>
            second.Bind(secondValue =>
                third.Map(thirdValue => selector(firstValue, secondValue, thirdValue))));
}