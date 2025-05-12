using System;

namespace CoreKernel.Functional.Results;

/// <summary>
/// Represents an error with a code, message, and type.
/// </summary>
/// <param name="Code">The unique code identifying the error.</param>
/// <param name="Message">The error message.</param>
/// <param name="Type">The type of the error.</param>
public record Error(string Code, string Message, ErrorType Type)
{
    /// <summary>
    /// Represents no error.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    /// <summary>
    /// Represents a null value error.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.", ErrorType.Failure);

    /// <summary>
    /// Represents a condition not met error.
    /// </summary>
    public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", "The specified condition was not met.", ErrorType.Failure);

    /// <summary>
    /// Represents a validation error.
    /// </summary>
    public static readonly Error ValidationError = new("Error.ValidationError", "A validation error occurred.", ErrorType.Validation);

    /// <summary>
    /// Represents an unauthorized request error.
    /// </summary>
    public static readonly Error UnauthorizedRequest = new("Error.Unauthorized", "An unauthorized error occurred.", ErrorType.Unauthorized);

    /// <summary>
    /// Creates a failure error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new failure error.</returns>
    /// <exception cref="ArgumentException">Thrown if the code or message is null or empty.</exception>
    public static Error Failure(string code, string message)
    {
        ValidateInput(code, message);
        return new Error(code, message, ErrorType.Failure);
    }

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    public static Error Validation(string code, string message)
    {
        ValidateInput(code, message);
        return new Error(code, message, ErrorType.Validation);
    }

    /// <summary>
    /// Appends additional details to the error message.
    /// </summary>
    /// <param name="details">The additional details to append.</param>
    /// <returns>A new error with the updated message.</returns>
    public Error WithDetails(string details)
    {
        return new Error(Code, $"{Message} - {details}", Type);
    }

    /// <summary>
    /// Validates the input parameters for error creation.
    /// </summary>
    private static void ValidateInput(string code, string message)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code cannot be null or empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message cannot be null or empty.", nameof(message));
    }
}

/// <summary>
/// Represents the type of error that can occur in the application.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// No error occurred.
    /// </summary>
    None,

    /// <summary>
    /// A general failure error.
    /// </summary>
    Failure,

    /// <summary>
    /// An error related to validation.
    /// </summary>
    Validation,

    /// <summary>
    /// An error indicating a conflict.
    /// </summary>
    Conflict,

    /// <summary>
    /// An error indicating a resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// An error indicating an unauthorized request.
    /// </summary>
    Unauthorized,

    /// <summary>
    /// An error indicating forbidden access.
    /// </summary>
    Forbidden,

    /// <summary>
    /// An error indicating the resource is gone.
    /// </summary>
    Gone,

    /// <summary>
    /// An error indicating no content is available.
    /// </summary>
    NoContent,

    /// <summary>
    /// An error indicating a bad request.
    /// </summary>
    BadRequest,

    /// <summary>
    /// An unexpected error occurred.
    /// </summary>
    Unexpected
}