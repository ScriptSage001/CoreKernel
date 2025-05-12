using System;
using CoreKernel.Functional.Results;

namespace CoreKernel.Functional.Validation;

/// <summary>
/// Represents the result of a validation operation, containing validation errors if the operation fails.
/// </summary>
/// <typeparam name="TValue">The type of the value associated with the result.</typeparam>
public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult{TValue}"/> class with the specified validation errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="errors"/> parameter is null.</exception>
    private ValidationResult(Error[] errors)
        : base(default, false, IValidationResult.ValidationError) =>
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));

    /// <summary>
    /// Gets the collection of validation errors associated with this result.
    /// </summary>
    public Error[] Errors { get; }

    /// <summary>
    /// Creates a new <see cref="ValidationResult{TValue}"/> instance with the specified validation errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    /// <returns>A new <see cref="ValidationResult{TValue}"/> instance containing the provided errors.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="errors"/> array is null or empty.</exception>
    public static ValidationResult<TValue> WithErrors(Error[] errors)
    {
        if (errors == null || errors.Length == 0)
            throw new ArgumentException("Errors cannot be null or empty.", nameof(errors));

        return new ValidationResult<TValue>(errors);
    }

    /// <summary>
    /// Matches the result to one of the provided functions based on its state.
    /// </summary>
    /// <typeparam name="TNextValue">The type of the value returned by the match functions.</typeparam>
    /// <param name="onSuccess">The function to execute if the result represents success.</param>
    /// <param name="onError">The function to execute if the result represents failure.</param>
    /// <returns>The value returned by the executed function.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="onSuccess"/> or <paramref name="onError"/> is null.
    /// </exception>
    public TNextValue Match<TNextValue>(Func<TValue, TNextValue> onSuccess, Func<Error[], TNextValue> onError)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onError);

        return IsSuccess ? onSuccess(Value) : onError(Errors);
    }
}