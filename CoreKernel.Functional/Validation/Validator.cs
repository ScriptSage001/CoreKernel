using System;
using System.Collections.Generic;
using System.Linq;
using CoreKernel.Functional.Results;

namespace CoreKernel.Functional.Validation;

/// <summary>
/// Provides methods for validating input against single or multiple validation rules.
/// </summary>
public static class Validator
{
    private const string ValidationErrorCode = "Error.ValidationError";
    
    /// <summary>
    /// Validates an input against a single rule.
    /// </summary>
    /// <typeparam name="T">The type of the input.</typeparam>
    /// <param name="input">The input to validate.</param>
    /// <param name="validationRule">The validation rule to apply.</param>
    /// <param name="errorMessage">The error message if validation fails.</param>
    /// <returns>A success result if valid; otherwise, a failure result.</returns>
    public static Result<T> Validate<T>(T input, Func<T, bool> validationRule, string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(validationRule);
        ArgumentNullException.ThrowIfNull(errorMessage);

        return validationRule(input)
            ? Result.Success(input)
            : Result.Failure<T>(Error.Validation(ValidationErrorCode, errorMessage));
    }

    /// <summary>
    /// Validates an input against multiple rules.
    /// </summary>
    /// <typeparam name="T">The type of the input.</typeparam>
    /// <param name="input">The input to validate.</param>
    /// <param name="validationRules">The collection of validation rules and their error messages.</param>
    /// <returns>A success result if all rules pass; otherwise, a failure result with errors.</returns>
    public static Result<T> Validate<T>(T input, IEnumerable<(Func<T, bool> rule, string errorMessage)> validationRules)
    {
        ArgumentNullException.ThrowIfNull(validationRules);

        var errors = validationRules
            .Where(r => !r.rule(input))
            .Select(r => Error.Validation(ValidationErrorCode, r.errorMessage))
            .ToArray();

        return errors.Length != 0
            ? ValidationResult<T>.WithErrors(errors)
            : Result.Success(input);
    }
}