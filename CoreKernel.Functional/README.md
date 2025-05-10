# CoreKernel.Functional Library

The `CoreKernel.Functional` library provides robust functional programming abstractions for C# applications, enabling more declarative, expressive, and error-resistant code patterns. It offers monadic types like `Result<T>` and `Maybe<T>` to handle common scenarios such as error propagation and optional values in a functional style.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Result Pattern](#result-pattern)
    - [Creating Results](#creating-results)
    - [Working with Results](#working-with-results)
    - [Chaining Operations](#chaining-operations)
  - [Maybe Pattern](#maybe-pattern)
    - [Creating Maybe Values](#creating-maybe-values)
    - [Working with Maybe Values](#working-with-maybe-values)
    - [Transforming Maybe Values](#transforming-maybe-values)
  - [Validation](#validation)
    - [Creating Validation Results](#creating-validation-results)
    - [Using Validation Results](#using-validation-results)
  - [Error Handling](#error-handling)
    - [Error Types](#error-types)
    - [Creating Custom Errors](#creating-custom-errors)
- [Integration Examples](#integration-examples)
- [Best Practices](#best-practices)

## Overview

The `CoreKernel.Functional` library provides a robust implementation of functional programming patterns for C#, focusing on:

- Explicit handling of failure cases through `Result<T>` types
- Null safety with `Maybe<T>` to represent optional values
- Comprehensive validation mechanisms with structured error reporting
- Extensive extension methods for fluent, expressive code

This approach leads to more maintainable code with fewer runtime exceptions and more explicit error handling.

## Features

### Result Pattern
- `Result` and `Result<T>` types for representing operation outcomes
- Composition and transformation methods (`Map`, `Bind`, etc.)
- Pattern matching with `Match` for handling different result states
- Extension methods for fluent operation chaining

### Maybe Pattern
- `Maybe<T>` type for safe handling of optional values
- Methods to transform and compose optional values
- Integration with the Result pattern via extension methods

### Validation
- `ValidationResult` types for collecting multiple validation errors
- Support for complex validation scenarios
- Integration with the Result pattern

### Error Handling
- Structured error representation with `Error` records
- Predefined error categories and types
- Support for custom error definitions

## Installation

Add the `CoreKernel.Functional` package to your project:

```bash
dotnet add package CoreKernel.Functional
```

## Usage

### Result Pattern

The Result pattern provides a way to represent the outcome of operations that might fail, without resorting to exceptions for control flow.

#### Creating Results

```csharp
using CoreKernel.Functional.Results;

// Create a success result with no value
Result successResult = Result.Success();

// Create a success result with a value
Result<int> successWithValue = Result.Success(42);

// Create a failure result with an error
Result failureResult = Result.Failure(Error.ValidationError);

// Create a failure result with a custom error message
Result<string> failureWithCustomError = Result.Failure<string>(
    Error.Failure("User.NotFound", "The specified user was not found.")
);

// Create a result based on a condition
Result conditionalResult = Result.Create(isValid);

// Create a result from a potentially null value
Result<User> userResult = Result.Create(user); // Returns Error.NullValue if user is null
```

#### Working with Results

```csharp
// Using OnSuccess and OnFailure for side effects
userResult
    .OnSuccess(user => LogUserAccess(user))
    .OnFailure(error => LogError(error));

// Matching result states
string message = userResult.Match(
    onSuccess: user => $"User {user.Name} was found",
    onFailure: result => $"Error: {result.Error.Message}"
);

// Transforming results with Map
Result<UserDto> userDtoResult = userResult.Map(user => new UserDto(user));

// Chaining dependent operations with Bind
Result<UserSettings> settingsResult = userResult.Bind(user => GetUserSettings(user.Id));
```

#### Chaining Operations

```csharp
// Example of chaining multiple operations
Result<OrderConfirmation> PlaceOrder(OrderRequest request) =>
    ValidateOrder(request)
        .Bind(order => CalculateTotals(order))
        .Bind(order => CheckInventory(order))
        .Bind(order => ProcessPayment(order))
        .Bind(order => SaveOrder(order))
        .Map(order => GenerateConfirmation(order));
```

### Maybe Pattern

The Maybe pattern provides a type-safe way to handle optional values, eliminating null reference exceptions.

#### Creating Maybe Values

```csharp
using CoreKernel.Functional.Maybe;

// Create a Maybe with a value
Maybe<string> someName = Maybe<string>.Some("John");

// Create an empty Maybe
Maybe<string> noName = Maybe<string>.None;

// Implicit conversion from value to Maybe
Maybe<int> maybeAge = 30; // Automatically converts to Some(30)
```

#### Working with Maybe Values

```csharp
// Check if a Maybe has a value
if (someName.HasValue)
{
    string name = someName.ValueOrThrow(); // Will not throw, as we checked HasValue
}

// Get value or default
string nameOrDefault = someName.ValueOrDefault() ?? "Anonymous";

// Use Match for handling both cases
string greeting = someName.Match(
    onSome: name => $"Hello, {name}!",
    onNone: () => "Hello, stranger!"
);

// Execute different actions based on presence of value
someName.Match(
    onSome: name => Console.WriteLine($"Name is {name}"),
    onNone: () => Console.WriteLine("No name provided")
);
```

#### Transforming Maybe Values

```csharp
// Transform a value if present with Map
Maybe<int> nameLength = someName.Map(name => name.Length);

// Chain Maybe-returning operations with Bind
Maybe<User> FindUserByEmail(string email) { /* ... */ }
Maybe<UserPreferences> GetUserPreferences(User user) { /* ... */ }

Maybe<string> userEmail = Maybe<string>.Some("user@example.com");
Maybe<UserPreferences> preferences = userEmail.Bind(FindUserByEmail).Bind(GetUserPreferences);

// Convert to Result using extension method
using CoreKernel.Functional.Extensions;

Result<string> nameResult = someName.ToResult("Name is required");
```

### Validation

The Validation types allow for collecting multiple validation errors in a structured way.

#### Creating Validation Results

```csharp
using CoreKernel.Functional.Validation;

// Create a validation result with multiple errors
ValidationResult validationResult = ValidationResult.WithErrors(new[]
{
    Error.Validation("User.Email.Invalid", "Email address is not in a valid format"),
    Error.Validation("User.Password.TooShort", "Password must be at least 8 characters")
});

// Create a typed validation result
ValidationResult<User> userValidationResult = ValidationResult<User>.WithErrors(new[]
{
    Error.Validation("User.NotFound", "User not found")
});
```

#### Using Validation Results

```csharp
// Handle validation result with different behavior for success and error
Result<User> CreateUser(CreateUserRequest request)
{
    var validationErrors = new List<Error>();
    
    if (string.IsNullOrEmpty(request.Email))
        validationErrors.Add(Error.Validation("User.Email.Required", "Email is required"));
        
    if (string.IsNullOrEmpty(request.Password))
        validationErrors.Add(Error.Validation("User.Password.Required", "Password is required"));
    
    if (validationErrors.Count > 0)
        return ValidationResult<User>.WithErrors(validationErrors.ToArray());
    
    // Create and return user if validation passes
    return Result.Success(new User(request.Email, request.Password));
}

// Using Match with ValidationResult
string message = userValidationResult.Match(
    onSuccess: user => $"User created: {user.Email}",
    onError: errors => $"Validation failed: {string.Join(", ", errors.Select(e => e.Message))}"
);
```

### Error Handling

The `Error` record type provides a structured way to represent errors across the application.

#### Error Types

```csharp
using CoreKernel.Functional.Results;

// Predefined errors
Error nullValueError = Error.NullValue;
Error validationError = Error.ValidationError;
Error unauthorizedError = Error.UnauthorizedRequest;

// Check error type
if (error.Type == ErrorType.Validation)
{
    // Handle validation error
}
```

#### Creating Custom Errors

```csharp
// Create custom errors with the factory methods
Error customError = Error.Failure(
    "Order.Processing.Failed", 
    "Failed to process the order due to payment issue"
);

Error validationError = Error.Validation(
    "Product.Price.Invalid",
    "Product price must be greater than zero"
);

// Add details to an existing error
Error detailedError = customError.WithDetails("Transaction ID: 1234567");
```

## Integration Examples

### Using with Entity Framework Core

```csharp
public async Task<Result<User>> GetUserByIdAsync(Guid id)
{
    var user = await _dbContext.Users.FindAsync(id);
    return Result.Create(user) // Returns failure with NullValue error if user is null
        .OnSuccess(u => _logger.LogInformation("User {UserId} retrieved", u.Id))
        .OnFailure(e => _logger.LogWarning("Failed to retrieve user {UserId}: {Error}", id, e.Message));
}
```

### Using with ASP.NET Core Controllers

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(Guid id)
{
    var result = await _userService.GetUserByIdAsync(id);
    
    return result.Match(
        onSuccess: user => Ok(user),
        onFailure: error => error.Type switch
        {
            ErrorType.NotFound => NotFound(error.Message),
            ErrorType.Unauthorized => Unauthorized(error.Message),
            _ => BadRequest(error.Message)
        }
    );
}
```

### Combining Multiple Operations

```csharp
public async Task<Result<OrderConfirmation>> PlaceOrderAsync(OrderRequest request)
{
    // Validate the order
    var validationResult = ValidateOrder(request);
    if (validationResult.IsFailure)
        return validationResult.Error;
    
    // Get the user
    var userResult = await GetUserByIdAsync(request.UserId);
    if (userResult.IsFailure)
        return userResult.Error;
    
    // Check user's payment methods
    var paymentMethodResult = await GetPaymentMethodAsync(userResult.Value, request.PaymentMethodId);
    if (paymentMethodResult.IsFailure)
        return paymentMethodResult.Error;
    
    // Process the payment
    var paymentResult = await ProcessPaymentAsync(paymentMethodResult.Value, request.TotalAmount);
    if (paymentResult.IsFailure)
        return paymentResult.Error;
    
    // Create and save the order
    var orderResult = await CreateOrderAsync(userResult.Value, request, paymentResult.Value);
    if (orderResult.IsFailure)
        return orderResult.Error;
    
    // Generate the confirmation
    return Result.Success(new OrderConfirmation(orderResult.Value.Id, request.TotalAmount));
}

// The same logic using Bind for more concise code:
public async Task<Result<OrderConfirmation>> PlaceOrderAsync(OrderRequest request)
{
    return ValidateOrder(request)
        .Bind(async _ => await GetUserByIdAsync(request.UserId))
        .Bind(async user => await GetPaymentMethodAsync(user, request.PaymentMethodId)
            .Map(paymentMethod => (user, paymentMethod)))
        .Bind(async tuple => await ProcessPaymentAsync(tuple.paymentMethod, request.TotalAmount)
            .Map(payment => (tuple.user, payment)))
        .Bind(async tuple => await CreateOrderAsync(tuple.user, request, tuple.payment))
        .Map(order => new OrderConfirmation(order.Id, request.TotalAmount));
}
```

## Best Practices

1. **Prefer `Result<T>` over exceptions** for expected error cases to make error handling explicit
2. **Use `Maybe<T>` instead of null** for optional values to prevent null reference exceptions
3. **Chain operations with `Bind` and `Map`** instead of using nested conditionals
4. **Use `Match` to ensure all cases are handled** rather than manual checking
5. **Create domain-specific errors** with descriptive codes and messages for better error reporting
6. **Combine validation errors** using `ValidationResult` to provide comprehensive feedback
7. **Keep `OnSuccess` and `OnFailure` side-effect-only** and use for logging or monitoring
8. **Return early for independent validations**, collect errors for related validations
9. **Use factory methods** (`Result.Success`, `Result.Failure`, etc.) rather than constructors
10. **Avoid mixing exception-based and Result-based error handling** in the same flow