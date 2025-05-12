using System;
using CoreKernel.Functional.Results;

namespace CoreKernel.Functional.Validation;

/// <summary>
/// Represents the result of a validation operation, containing validation errors if the operation fails.
/// </summary>
public sealed class ValidationResult : Result, IValidationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class with the specified validation errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="errors"/> parameter is null.</exception>
    private ValidationResult(Error[] errors)
        : base(false, IValidationResult.ValidationError) =>
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));

    /// <summary>
    /// Gets the collection of validation errors associated with this result.
    /// </summary>
    public Error[] Errors { get; }

    /// <summary>
    /// Creates a new <see cref="ValidationResult"/> instance with the specified validation errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    /// <returns>A new <see cref="ValidationResult"/> instance containing the provided errors.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="errors"/> array is null or empty.</exception>
    public static ValidationResult WithErrors(Error[] errors)
    {
        if (errors == null || errors.Length == 0)
            throw new ArgumentException("Errors cannot be null or empty.", nameof(errors));

        return new ValidationResult(errors);
    }
}