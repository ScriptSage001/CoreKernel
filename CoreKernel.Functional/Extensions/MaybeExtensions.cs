using CoreKernel.Functional.Maybe;
using CoreKernel.Functional.Results;

namespace CoreKernel.Functional.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="Maybe{T}"/> instances.
/// </summary>
public static class MaybeExtensions
{
    /// <summary>
    /// Converts a <see cref="Maybe{T}"/> instance to a <see cref="Result{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the <see cref="Maybe{T}"/>.</typeparam>
    /// <param name="maybe">The <see cref="Maybe{T}"/> instance to convert.</param>
    /// <param name="errorMessage">The error message to use if the <see cref="Maybe{T}"/> has no value.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> instance representing success if the <see cref="Maybe{T}"/> has a value,
    /// or failure with the specified error message if it does not.
    /// </returns>
    public static Result<T> ToResult<T>(this Maybe<T> maybe, string errorMessage)
    {
        // Check if the Maybe instance has a value.
        // If it does, return a successful Result with the value.
        // Otherwise, return a failure Result with the provided error message.
        return maybe.HasValue
            ? Result.Success(maybe.ValueOrThrow())
            : Result.Failure<T>(Error.Failure("Error.NullValue", errorMessage));
    }

    /// <summary>
    /// Converts a <see cref="Result{T}"/> instance to a <see cref="Maybe{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the <see cref="Result{T}"/>.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> instance to convert.</param>
    /// <returns>
    /// A <see cref="Maybe{T}"/> instance representing some value if the <see cref="Result{T}"/> is successful,
    /// or none if the <see cref="Result{T}"/> is a failure.
    /// </returns>
    public static Maybe<T> ToMaybe<T>(this Result<T> result)
    {
        // Check if the Result instance is successful.
        // If it is, return a Maybe instance containing the value.
        // Otherwise, return a Maybe instance representing no value.
        return result.IsSuccess 
            ? Maybe<T>.Some(result.Value) 
            : Maybe<T>.None;
    }
}