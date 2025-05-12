# CoreKernel.Messaging Library

The `CoreKernel.Messaging` library provides essential abstractions for implementing the Command Query Responsibility Segregation (CQRS) and Event-Driven Architecture patterns in C#. It leverages MediatR to offer a clean messaging infrastructure for commands, queries, and events, empowering developers to build maintainable and scalable applications.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Commands](#commands)
  - [Queries](#queries)
  - [Events](#events)
  - [Domain Events](#domain-events)
  - [Event Publishing](#event-publishing)
- [Best Practices](#best-practices)

---

## Overview

The `CoreKernel.Messaging` project encapsulates the CQRS pattern's core concepts as reusable abstractions. It provides a clean separation between write operations (commands), read operations (queries), and notifications (events). By building on top of MediatR, it enables a decoupled architecture where handlers process specific message types, leading to better testability and maintainability.

---

## Features

- **Command Infrastructure**
  - Supports commands with and without return values
  - Wraps responses in a `Result` type for consistent error handling

- **Query Infrastructure**
  - Clear abstractions for read operations
  - Type-safe response handling with `Result<T>`

- **Event System**
  - Base event interface with tracking properties (Id, TimeStamp, CorrelationId)
  - Support for multiple subscribers to a single event

- **Domain Event Integration**
  - Specialized interfaces for domain events
  - Seamless integration with the `CoreKernel.Primitives` aggregates

- **Event Publishing**
  - Abstraction for publishing events across the system

---

## Installation

Add the `CoreKernel.Messaging` package to your project:

```bash
dotnet add package CoreKernel.Messaging
```

---

## Usage

### Commands

Commands represent intentions to change the system state. The library supports commands with and without return values:

```csharp
// Command without specific return value
public class CreateProductCommand : ICommand
{
    public string Name { get; init; }
    public decimal Price { get; init; }
}

// Command handler
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    public async Task<Result> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Implementation that creates a product
        return Result.Success();
    }
}
```

Commands with return values:

```csharp
// Command with return value
public class RegisterUserCommand : ICommand<Guid>
{
    public string Username { get; init; }
    public string Email { get; init; }
}

// Command handler with return value
public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        // Implementation that registers a user
        var userId = Guid.NewGuid();
        return Result.Success(userId);
    }
}
```

---

### Queries

Queries retrieve data without changing system state:

```csharp
// Query definition
public class GetProductByIdQuery : IQuery<ProductDto>
{
    public Guid Id { get; init; }
}

// Query handler
public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        // Implementation that retrieves product data
        return Result.Success(new ProductDto { /* ... */ });
    }
}
```

---

### Events

Events represent notifications about something that has happened:

```csharp
// Event definition
public class UserRegisteredEvent : IEvent
{
    public Guid Id { get; init; }
    public DateTime TimeStamp { get; init; }
    public Guid CorrelationId { get; init; }
    
    public string Username { get; init; }
    public string Email { get; init; }
}

// Event handler
public class SendWelcomeEmailHandler : IEventHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        // Implementation to send welcome email
    }
}
```

Multiple handlers can subscribe to the same event:

```csharp
public class UpdateUserStatisticsHandler : IEventHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        // Implementation to update statistics
    }
}
```

---

### Domain Events

Domain events are specialized events that represent significant occurrences within the domain:

```csharp
// Domain event definition
public class OrderPlacedEvent : IDomainEvent
{
    public Guid Id { get; init; }
    public DateTime TimeStamp { get; init; }
    public Guid CorrelationId { get; init; }
    
    public Guid OrderId { get; init; }
    public decimal TotalAmount { get; init; }
}

// Domain event handler
public class UpdateInventoryHandler : IDomainEventHandler<OrderPlacedEvent>
{
    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        // Implementation to update inventory
    }
}
```

---

### Event Publishing

The `IEventPublisher` interface provides an abstraction for publishing events:

```csharp
public class EventPublisherService
{
    private readonly IEventPublisher _eventPublisher;
    
    public EventPublisherService(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }
    
    public async Task ProcessOrder(Order order)
    {
        // Business logic
        
        // Publish event
        await _eventPublisher.PublishAsync(new OrderProcessedEvent
        {
            Id = Guid.NewGuid(),
            TimeStamp = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
            OrderId = order.Id
        });
    }
}
```

---

## Best Practices

1. **Use Commands for Write Operations** - Any operation that modifies state should be modeled as a command.
2. **Use Queries for Read Operations** - State retrieval should be modeled as queries.
3. **Return Results, Not Exceptions** - Use the `Result` type to indicate success or failure instead of throwing exceptions.
4. **Keep Commands and Queries Simple** - They should be data containers without complex logic.
5. **Single Responsibility for Handlers** - Each handler should do one thing well.
6. **Use Domain Events for Cross-Aggregate Communication** - They maintain loose coupling between aggregates.
7. **Include Correlation IDs** - This aids in tracking operations across distributed systems.
8. **Make Events Immutable** - Use `init` properties to ensure events cannot be modified after creation.
9. **Consider Event Sourcing** - For systems where historical state is important.
10. **Maintain Command-Query Separation** - Don't mix state changes and data retrieval in the same operation.
