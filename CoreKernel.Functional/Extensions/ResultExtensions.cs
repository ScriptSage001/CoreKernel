using CoreKernel.Functional.Results;

namespace CoreKernel.Functional.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="Result"/> and <see cref="Result{T}"/> instances.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Matches the result to one of the provided functions based on its state.
    /// </summary>
    /// <typeparam name="TOut">The type of the value returned by the match functions.</typeparam>
    /// <param name="result">The <see cref="Result"/> instance to match.</param>
    /// <param name="onSuccess">The function to execute if the result represents success.</param>
    /// <param name="onFailure">The function to execute if the result represents failure.</param>
    /// <returns>The value returned by the executed function.</returns>
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<Result, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result);
    }

    /// <summary>
    /// Matches the result to one of the provided functions based on its state.
    /// </summary>
    /// <typeparam name="TIn">The type of the value contained in the <see cref="Result{TIn}"/>.</typeparam>
    /// <typeparam name="TOut">The type of the value returned by the match functions.</typeparam>
    /// <param name="result">The <see cref="Result{TIn}"/> instance to match.</param>
    /// <param name="onSuccess">The function to execute if the result represents success.</param>
    /// <param name="onFailure">The function to execute if the result represents failure.</param>
    /// <returns>The value returned by the executed function.</returns>
    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Result<TIn>, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
    }
}