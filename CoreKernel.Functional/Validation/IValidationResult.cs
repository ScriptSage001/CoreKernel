using CoreKernel.Functional.Results;

namespace CoreKernel.Functional.Validation;

/// <summary>
/// Represents a validation result interface that provides access to validation errors.
/// </summary>
public interface IValidationResult
{
    /// <summary>
    /// A predefined validation error instance.
    /// </summary>
    public static readonly Error ValidationError = Error.ValidationError;

    /// <summary>
    /// Gets the collection of validation errors associated with the result.
    /// </summary>
    Error[] Errors { get; }
}