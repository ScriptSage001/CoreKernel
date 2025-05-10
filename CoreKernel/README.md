# CoreKernel Library

The `CoreKernel` library provides a set of foundational abstractions and utilities for building robust, modular, and maintainable applications. It includes support for functional programming constructs, messaging patterns, error handling, and domain-driven design (DDD) principles.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
    - [Functional Results](#functional-results)
        - [Result and Result<T>](#result-and-resultt)
        - [Maybe](#maybe)
        - [Result Extensions](#result-extensions)
        - [Maybe Extensions](#maybe-extensions)
        - [Error](#error)
    - [Messaging](#messaging)
        - [Commands](#commands)
        - [Queries](#queries)
        - [Events](#events)
    - [Domain-Driven Design Primitives](#domain-driven-design-primitives)
        - [Entities](#entities)
        - [Aggregate Roots](#aggregate-roots)
        - [Value Objects](#value-objects)
        - [Strongly-Typed IDs](#strongly-typed-ids)
        - [Auditable Entities](#auditable-entities)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

The `CoreKernel` library is designed to simplify application development by providing:

- Functional programming constructs like `Result` and `Maybe`.
- A messaging framework for commands, queries, and events.
- Strongly-typed error handling with extensible error types.
- Domain-driven design (DDD) building blocks such as entities, value objects, and aggregate roots.

---

## Features

### Functional Results
- `Result` and `Result<T>` for handling success and failure states.
- `Maybe<T>` for representing optional values.
- Predefined error types and utilities for creating custom errors.

### Messaging
- Command and query abstractions using the MediatR library.
- Event-driven architecture with domain events and event handlers.

### Domain-Driven Design
- Base classes for entities, aggregate roots, and value objects.
- Support for strongly-typed identifiers and auditable entities.

---

## Installation

To use the `CoreKernel` library in your project, add the necessary references to your `.csproj` file or install the library via NuGet (if published).

```bash
dotnet add package CoreKernel
```

---

## Usage

### Functional Results

#### Result and Result<T>
The `Result` and `Result<T>` types are used to represent the outcome of operations. They can encapsulate success or failure states.
Example: Creating and Handling Results

```csharp
using CoreKernel.Functional.Results;

var successResult = Result.Success();
var failureResult = Result.Failure(Error.ValidationError);

successResult.Match(
    onSuccess: () => Console.WriteLine("Operation succeeded."),
    onFailure: error => Console.WriteLine($"Operation failed: {error.Message}")
);
```

#### Maybe
The `Maybe<T>` type represents an optional value that may or may not be present. It is useful for avoiding null references and expressing the absence of a value.

Example: Using `Maybe<T>`

```csharp
using CoreKernel.Functional.Maybe;
using CoreKernel.Functional.Extensions;

Maybe<int> maybeValue = Maybe<int>.Some(42);
var result = maybeValue.ToResult("Value is missing.");
```

#### Result Extensions
The `ResultExtensions` class provides additional functionality for working with `Result<T>` instances.
Example: Converting `Result<T>` to `Maybe<T>`

```csharp
using CoreKernel.Functional.Results;
using CoreKernel.Functional.Extensions;

var successResult = Result.Success();
var failureResult = Result.Failure(Error.ValidationError);

// Example: Using Match with Result
successResult.Match(
    onSuccess: () => Console.WriteLine("Operation succeeded."),
    onFailure: error => Console.WriteLine($"Operation failed: {error}")
);

// Example: Using Match with Result<T>
var resultWithValue = Result.Success(42);
var output = resultWithValue.Match(
    onSuccess: value => $"Success with value: {value}",
    onFailure: error => $"Failure: {error}"
);

Console.WriteLine(output);
```

#### Maybe Extensions
The `MaybeExtensions` class provides additional functionality for working with `Maybe<T>` instances.
Example: Converting `Maybe<T>` to `Result<T>`

```csharp
using CoreKernel.Functional.Maybe;
using CoreKernel.Functional.Extensions;

Maybe<string> maybeName = Maybe<string>.None;
var result = maybeName.ToResult("Name is required.");
```

#### Error
The `Error` class provides a set of predefined error types and a mechanism for creating custom errors.

Example: Creating Custom Errors

```csharp
using CoreKernel.Functional.Results;

var customError = new Error("Error.Custom", "A custom error occurred.", ErrorType.Unexpected);
Console.WriteLine($"Code: {customError.Code}, Message: {customError.Message}, Type: {customError.Type}");
```

---

### Messaging
The messaging framework is built on top of MediatR and provides abstractions for commands, queries, and events.  

#### Commands
Commands represent actions or operations in the system.

```csharp
using CoreKernel.Messaging.Commands;

public class CreateUserCommand : ICommand
{
    public string UserName { get; set; }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Handle the command logic
        return Task.FromResult(Result.Success());
    }
}
```

##### Queries
Queries are used to retrieve data or information.

```csharp
using CoreKernel.Messaging.Queries;

public class GetUserQuery : IQuery<User>
{
    public Guid UserId { get; set; }
}

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, User>
{
    public Task<Result<User>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        // Handle the query logic
        return Task.FromResult(Result.Success(new User { Id = query.UserId, Name = "John Doe" }));
    }
}
```

#### Events
Events represent significant occurrences in the system.

```csharp
using CoreKernel.Messaging.Events;

public class UserCreatedEvent : IDomainEvent
{
    public Guid Id { get; init; }
    public DateTime TimeStamp { get; init; }
    public Guid CorrelationId { get; init; }
}

public class UserCreatedEventHandler : IDomainEventHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        // Handle the event logic
        return Task.CompletedTask;
    }
}
```

### Domain-Driven Design Primitives

#### Entities
The `Entity<TId>` base class provides a foundation for domain entities with a unique identifier and equality checks.

```csharp
public class User : Entity<Guid>
{
    public string Name { get; set; }

    public User(Guid id, string name) : base(id)
    {
        Name = name;
    }
}
```

#### Aggregate Roots
The AggregateRoot<TId> class extends Entity<TId> and adds support for domain events.

```csharp
public class Order : AggregateRoot<Guid>
{
    public Order(Guid id) : base(id) { }

    public void AddItem(string item)
    {
        // Business logic
        RaiseDomainEvent(new OrderItemAddedEvent(id, item));
    }
}
```

#### Value Objects
The ValueObject base class simplifies the implementation of value objects by providing equality comparison based on their properties.

```csharp
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }

    public Address(string street, string city)
    {
        Street = street;
        City = city;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Street;
        yield return City;
    }
}
```

#### Strongly-Typed IDs
The StronglyTypedId<T> class enforces type safety for identifiers.

```csharp
public class OrderId : StronglyTypedId<Guid>
{
    public OrderId(Guid value) : base(value) { }
}
```

#### Auditable Entities
The IAuditable interface provides properties for tracking creation and modification metadata.

```csharp
public class AuditableEntity : Entity<Guid>, IAuditable
{
    public Guid CorrelationId { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }
}
```

---

## Contributing
Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## License
This project is licensed under the Apache License 2.0. See the LICENSE file for details.