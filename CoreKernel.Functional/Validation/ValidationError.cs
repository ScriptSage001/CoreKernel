using System;
using System.Collections.Generic;
using System.Linq;
using CoreKernel.Functional.Results;

namespace CoreKernel.Functional.Validation;

/// <summary>
/// Represents a validation error that contains a collection of errors.
/// </summary>
/// <param name="Errors">The collection of validation errors.</param>
public sealed record ValidationError(Error[] Errors)
    : Error(
        "Validation.General",
        "One or more validation errors occurred",
        ErrorType.Validation)
{
    /// <summary>
    /// Creates a new <see cref="ValidationError"/> from a collection of failed results.
    /// </summary>
    /// <param name="results">The collection of results to extract errors from.</param>
    /// <returns>A new <see cref="ValidationError"/> instance containing the extracted errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="results"/> parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown if no errors are found in the provided results.</exception>
    public static ValidationError FromResults(IEnumerable<Result> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var errors = results.Where(r => r.IsFailure).Select(r => r.Error).ToArray();
        if (errors.Length == 0)
            throw new ArgumentException("No errors found in the provided results.", nameof(results));

        return new ValidationError(errors);
    }
}