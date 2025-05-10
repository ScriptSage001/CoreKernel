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
        return maybe.HasValue
            ? Result.Success(maybe.ValueOrThrow())
            : Result.Failure<T>(Error.Failure("Error.NullValue", errorMessage));
    }
}