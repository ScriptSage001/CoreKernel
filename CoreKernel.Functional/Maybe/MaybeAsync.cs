using System;
using System.Threading.Tasks;

namespace CoreKernel.Functional.Maybe;

/// <summary>
/// Provides asynchronous extension methods for <see cref="Maybe{T}"/>.
/// </summary>
public static class MaybeAsync
{
    /// <summary>
    /// Creates a Maybe from a Task that may return null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The task that may return a value.</param>
    /// <returns>A task that returns a Maybe containing the value if non-null, or None if null.</returns>
    public static async Task<Maybe<T>> ToMaybeAsync<T>(this Task<T> task)
    {
        var result = await task.ConfigureAwait(false);
        return result is null ? Maybe<T>.None : Maybe<T>.Some(result);
    }

    /// <summary>
    /// Maps the value in a Maybe using an asynchronous mapping function.
    /// </summary>
    /// <typeparam name="T">The source type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="maybe">The Maybe to map.</param>
    /// <param name="mapperAsync">The asynchronous mapping function.</param>
    /// <returns>A task that returns a new Maybe containing the mapped value if present, or None if not.</returns>
    public static Task<Maybe<TResult>> MapAsync<T, TResult>(
        this Maybe<T> maybe,
        Func<T, Task<TResult>> mapperAsync)
    {
        return maybe.Match(
            async value => Maybe<TResult>.Some(await mapperAsync(value).ConfigureAwait(false)),
            () => Task.FromResult(Maybe<TResult>.None));
    }

    /// <summary>
    /// Binds the value in a Maybe using an asynchronous binding function.
    /// </summary>
    /// <typeparam name="T">The source type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="maybe">The Maybe to bind.</param>
    /// <param name="binderAsync">The asynchronous binding function.</param>
    /// <returns>A task that returns the bound Maybe, or None if the source contains no value.</returns>
    public static Task<Maybe<TResult>> BindAsync<T, TResult>(
        this Maybe<T> maybe,
        Func<T, Task<Maybe<TResult>>> binderAsync)
    {
        return maybe.Match(
            binderAsync,
            () => Task.FromResult(Maybe<TResult>.None));
    }

    /// <summary>
    /// Matches a Maybe to two asynchronous functions based on whether it contains a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="maybe">The Maybe to match.</param>
    /// <param name="onSomeAsync">The function to execute if a value is present.</param>
    /// <param name="onNoneAsync">The function to execute if no value is present.</param>
    /// <returns>A task that returns the result of the executed function.</returns>
    public static Task<TResult> MatchAsync<T, TResult>(
        this Maybe<T> maybe,
        Func<T, Task<TResult>> onSomeAsync,
        Func<Task<TResult>> onNoneAsync)
    {
        return maybe.Match(
            onSomeAsync,
            onNoneAsync);
    }

    /// <summary>
    /// Executes an asynchronous action on the value if present.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="maybe">The Maybe to operate on.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task that completes when the action completes.</returns>
    public static Task DoAsync<T>(this Maybe<T> maybe, Func<T, Task> action)
    {
        return maybe.Match(
            async value => { await action(value).ConfigureAwait(false); },
            () => Task.CompletedTask);
    }

    /// <summary>
    /// Converts a Maybe to a Task of "Maybe".
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="maybe">The Maybe to convert.</param>
    /// <returns>A task that returns the "Maybe".</returns>
    public static Task<Maybe<T>> AsTask<T>(this Maybe<T> maybe)
    {
        return Task.FromResult(maybe);
    }
}