# CoreKernel Library

The `CoreKernel` library provides a comprehensive set of foundational abstractions and utilities for building robust, modular, and maintainable .NET applications. It implements patterns from Domain-Driven Design (DDD), functional programming, and modern architectural approaches like CQRS and Event-Driven Architecture.

## Table of Contents

- [Overview](#overview)
- [Components](#components)
- [Installation](#installation)
- [Usage](#usage)
    - [Domain-Driven Design Primitives](#domain-driven-design-primitives)
        - [Entities](#entities)
        - [Aggregate Roots](#aggregate-roots)
        - [Value Objects](#value-objects)
        - [Strongly-Typed IDs](#strongly-typed-ids)
    - [Domain Markers](#domain-markers)
        - [Auditing](#auditing)
        - [Multi-Tenancy](#multi-tenancy)
        - [Soft Deletion](#soft-deletion)
        - [Tracing](#tracing)
    - [Functional Programming](#functional-programming)
        - [Result Pattern](#result-pattern)
        - [Maybe Pattern](#maybe-pattern)
        - [Validation](#validation)
        - [Error Handling](#error-handling)
    - [Messaging](#messaging)
        - [Commands](#commands)
        - [Queries](#queries)
        - [Events](#events)
        - [Domain Events](#domain-events)
- [Integration Examples](#integration-examples)
- [Best Practices](#best-practices)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

The `CoreKernel` library is a comprehensive toolkit designed to simplify application development by providing:

- **DDD building blocks** for modeling your business domain with clarity and precision
- **Functional programming constructs** for safer, more expressive code
- **Cross-cutting concerns** handled through domain markers
- **Messaging infrastructure** for implementing CQRS and event-driven architecture
- **Consistent error handling** through the Result pattern

By adopting these patterns early in your development process, you can ensure better code maintainability, domain integrity, and system scalability.

## Components

The `CoreKernel` library consists of the following components:

- **CoreKernel.Primitives**: Domain-Driven Design building blocks (entities, value objects, aggregates)
- **CoreKernel.DomainMarkers**: Interfaces for cross-cutting concerns (auditing, multi-tenancy, etc.)
- **CoreKernel.Functional**: Functional programming abstractions (Result, Maybe, Error types)
- **CoreKernel.Messaging**: CQRS and event-driven architecture support (commands, queries, events)

Each component can be used independently or together for a complete development experience.

## Installation

To use the full `CoreKernel` library in your project:

```bash
dotnet add package CoreKernel
```

Or, to use individual components:

```bash
dotnet add package CoreKernel.Primitives
dotnet add package CoreKernel.DomainMarkers
dotnet add package CoreKernel.Functional
dotnet add package CoreKernel.Messaging
```

## Usage

### Domain-Driven Design Primitives

#### Entities

The `Entity<TId>` base class provides identity-based equality comparison:

```csharp
public class Product : Entity<Guid>
{
    public string Name { get; }
    public decimal Price { get; private set; }

    public Product(Guid id, string name, decimal price) : base(id)
    {
        Name = name;
        Price = price;
    }

    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
    }
}
```

Entities are compared by ID and type:

```csharp
var product1 = new Product(Guid.NewGuid(), "Widget", 10.99m);
var product2 = new Product(product1.Id, "Widget", 10.99m);

bool areEqual = product1 == product2; // true, same ID
```

#### Aggregate Roots

Aggregate roots are the entry points to domain aggregates and can raise domain events:

```csharp
public class Order : AggregateRoot<Guid>
{
    private readonly List<OrderLine> _orderLines = new();
    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
    
    public decimal TotalAmount { get; private set; }

    public Order(Guid id) : base(id) 
    {
        TotalAmount = 0;
    }

    public void AddOrderLine(Product product, int quantity)
    {
        var orderLine = new OrderLine(Guid.NewGuid(), product.Id, product.Price, quantity);
        _orderLines.Add(orderLine);
        
        TotalAmount += orderLine.LineTotal;
        
        RaiseDomainEvent(new OrderLineAddedEvent(Id, orderLine.Id));
    }

    public void Place()
    {
        // Business logic for placing an order
        RaiseDomainEvent(new OrderPlacedEvent(Id));
    }
}
```

Working with domain events:

```csharp
// Create and manipulate the order
var order = new Order(Guid.NewGuid());
order.AddOrderLine(product, 2);
order.Place();

// Retrieve domain events for processing
var events = order.GetDomainEvents();
// Process events...
order.ClearDomainEvents(); // Clear after processing
```

#### Value Objects

Value objects encapsulate concepts that are distinguished by their attributes rather than identity:

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
            
        return new Money(Amount + other.Amount, Currency);
    }
}
```

Value objects are compared by their structure, not reference:

```csharp
var price1 = new Money(100, "USD");
var price2 = new Money(100, "USD");

bool areEqual = price1 == price2; // true, same values
```

#### Strongly-Typed IDs

Strongly-typed IDs prevent accidental ID misuse between different entity types:

```csharp
public class UserId : StronglyTypedId<Guid>
{
    public UserId(Guid value) : base(value) { }
}

public class OrderId : StronglyTypedId<Guid>
{
    public OrderId(Guid value) : base(value) { }
}

public class User : Entity<UserId>
{
    public string Name { get; }
    
    public User(UserId id, string name) : base(id)
    {
        Name = name;
    }
}
```

This prevents accidentally passing an OrderId to a method expecting a UserId.

### Domain Markers

#### Auditing

Auditing interfaces track entity creation and modification:

```csharp
// Basic time tracking
public class BlogPost : Entity<Guid>, ITimeStamped
{
    public string Title { get; set; }
    public string Content { get; set; }
    
    // ITimeStamped implementation
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset LastModifiedOn { get; set; }
}

// Complete auditing with user tracking
public class Invoice : Entity<Guid>, IAuditable
{
    public decimal Amount { get; set; }
    public string CustomerName { get; set; }
    
    // IAuditable implementation
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset LastModifiedOn { get; set; }
    public string LastModifiedBy { get; set; }
}
```

#### Multi-Tenancy

The `ITenantScoped<TId>` interface enables multi-tenant data isolation:

```csharp
public class CustomerRecord : Entity<Guid>, ITenantScoped<Guid>
{
    public string CustomerName { get; set; }
    public string ContactEmail { get; set; }
    
    // ITenantScoped implementation
    public Guid TenantId { get; set; }
}
```

This allows for automatic tenant filtering in queries.

#### Soft Deletion

The `ISoftDeletable` interface supports logical deletion of entities:

```csharp
public class Document : Entity<Guid>, ISoftDeletable
{
    public string Title { get; set; }
    public string Content { get; set; }
    
    // ISoftDeletable implementation
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
}
```

#### Tracing

The `ITraceable` interface enables distributed tracing:

```csharp
public class Payment : Entity<Guid>, ITraceable
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    
    // ITraceable implementation
    public Guid CorrelationId { get; set; }
    public string? TraceSource { get; set; }
    public string? OperationName { get; set; }
}
```

### Functional Programming

#### Result Pattern

The Result pattern provides a way to represent operation outcomes that might fail:

```csharp
// Creating results
Result successResult = Result.Success();
Result<int> successWithValue = Result.Success(42);
Result failureResult = Result.Failure(Error.ValidationError);
Result<string> failureWithCustomError = Result.Failure<string>(
    Error.Failure("User.NotFound", "The specified user was not found.")
);

// Pattern matching
string message = userResult.Match(
    onSuccess: user => $"User {user.Name} was found",
    onFailure: error => $"Error: {error.Message}"
);

// Transforming results
Result<UserDto> userDtoResult = userResult.Map(user => new UserDto(user));

// Chaining operations
Result<OrderConfirmation> confirmationResult = ValidateOrder(request)
    .Bind(order => CalculateTotals(order))
    .Bind(order => CheckInventory(order))
    .Bind(order => ProcessPayment(order))
    .Map(order => GenerateConfirmation(order));
```

#### Maybe Pattern

The Maybe pattern provides type-safe handling of optional values:

```csharp
// Creating Maybe values
Maybe<string> someName = Maybe<string>.Some("John");
Maybe<string> noName = Maybe<string>.None;

// Using Match for handling both cases
string greeting = someName.Match(
    onSome: name => $"Hello, {name}!",
    onNone: () => "Hello, stranger!"
);

// Transforming Maybe values
Maybe<int> nameLength = someName.Map(name => name.Length);

// Converting to Result
Result<string> nameResult = someName.ToResult("Name is required");
```

#### Validation

##### Validator Overview

The `Validator` class provides methods to validate input against single or multiple rules, returning structured results. It integrates seamlessly with the `Result` and `ValidationResult` patterns.

###### Validating Single Rule

```csharp
using CoreKernel.Functional.Validation;

// Validate input against a single rule
var result = Validator.Validate(
    input: "example@example.com",
    validationRule: value => value.Contains("@"),
    errorMessage: "Input must contain '@'."
);

if (result.IsSuccess)
{
    Console.WriteLine("Validation succeeded: " + result.Value);
}
else
{
    Console.WriteLine("Validation failed: " + result.Error.Message);
}
```
###### Validating Multiple Rules

```csharp
using CoreKernel.Functional.Validation;

// Define validation rules
var validationRules = new List<(Func<string, bool> rule, string errorMessage)>
{
  (value => !string.IsNullOrWhiteSpace(value), "Input cannot be empty."),
  (value => value.Contains("@"), "Input must contain '@'."),
  (value => value.Length <= 50, "Input must not exceed 50 characters.")
};

// Validate input against multiple rules
var result = Validator.Validate("example@example.com", validationRules);

if (result.IsSuccess)
{
    Console.WriteLine("Validation succeeded: " + result.Value);
}
else
{
    Console.WriteLine("Validation failed with errors:");
    foreach (var error in ((IValidationResult)result).Errors)
    {
        Console.WriteLine($"- {error.Message}");
    }
}
```

Validation types allow collecting multiple validation errors:

```csharp
// Creating validation results
ValidationResult validationResult = ValidationResult.WithErrors(new[]
{
    Error.Validation("User.Email.Invalid", "Email address is not in a valid format"),
    Error.Validation("User.Password.TooShort", "Password must be at least 8 characters")
});

// Using Match with ValidationResult
string message = userValidationResult.Match(
    onSuccess: user => $"User created: {user.Email}",
    onError: errors => $"Validation failed: {string.Join(", ", errors.Select(e => e.Message))}"
);
```

#### Error Handling

The `Error` record type provides structured error representation:

```csharp
// Predefined errors
Error nullValueError = Error.NullValue;
Error validationError = Error.ValidationError;

// Custom errors
Error customError = Error.Failure(
    "Order.Processing.Failed", 
    "Failed to process the order due to payment issue"
);

// Adding details
Error detailedError = customError.WithDetails("Transaction ID: 1234567");
```

### Messaging

#### Commands

Commands represent intentions to change system state:

```csharp
// Command definition
public class CreateProductCommand : ICommand<Guid>
{
    public string Name { get; init; }
    public decimal Price { get; init; }
}

// Command handler
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Implementation to create a product
        var productId = Guid.NewGuid();
        return Result.Success(productId);
    }
}
```

#### Queries

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
        // Implementation to retrieve product data
        return Result.Success(new ProductDto { /* ... */ });
    }
}
```

#### Events

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

#### Domain Events

Domain events are specialized events raised within the domain model:

```csharp
// Domain event definition
public class OrderPlacedEvent : IDomainEvent
{
    public Guid Id { get; init; }
    public DateTime TimeStamp { get; init; }
    public Guid CorrelationId { get; init; }
    
    public Guid OrderId { get; init; }
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

## Integration Examples

### Repository with Domain Markers

```csharp
public class GenericRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly ITenantProvider _tenantProvider;
    
    public async Task<Result<TEntity>> GetByIdAsync(TId id)
    {
        var entity = await _dbContext.Set<TEntity>().FindAsync(id);
        return entity != null 
            ? Result.Success(entity) 
            : Result.Failure<TEntity>(Error.NotFound($"{typeof(TEntity).Name} with ID {id} was not found."));
    }
    
    public async Task<Result<TId>> SaveAsync(TEntity entity)
    {
        // Handle audit information
        if (entity is IAuditable auditable)
        {
            var now = DateTimeOffset.UtcNow;
            var userId = _currentUser.UserId ?? "system";
            
            if (IsNew(entity))
            {
                auditable.CreatedOn = now;
                auditable.CreatedBy = userId;
            }
            
            auditable.LastModifiedOn = now;
            auditable.LastModifiedBy = userId;
        }
        
        // Handle multi-tenancy
        if (entity is ITenantScoped<Guid> tenantScoped && IsNew(entity))
        {
            tenantScoped.TenantId = _tenantProvider.GetCurrentTenantId();
        }
        
        // Handle tracing if applicable
        if (entity is ITraceable traceable)
        {
            traceable.CorrelationId = _currentUser.CorrelationId;
            traceable.TraceSource = GetType().Name;
            traceable.OperationName = IsNew(entity) ? "Create" : "Update";
        }
        
        try
        {
            if (IsNew(entity)) 
                _dbContext.Add(entity);
            
            await _dbContext.SaveChangesAsync();
            return Result.Success(entity.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<TId>(
                Error.Failure("Database.SaveFailed", $"Failed to save {typeof(TEntity).Name}: {ex.Message}")
            );
        }
    }
    
    public async Task<Result> DeleteAsync(TEntity entity)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.IsDeleted = true;
            softDeletable.DeletedOn = DateTimeOffset.UtcNow;
            softDeletable.DeletedBy = _currentUser.UserId ?? "system";
            
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        
        _dbContext.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return Result.Success();
    }
    
    private bool IsNew(TEntity entity)
    {
        return !_dbContext.Entry(entity).IsKeySet;
    }
}
```

### ASP.NET Core Controller with CQRS

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        
        return result.Match(
            onSuccess: product => Ok(product),
            onFailure: error => error.Type switch
            {
                ErrorType.NotFound => NotFound(error.Message),
                ErrorType.Unauthorized => Unauthorized(error.Message),
                _ => BadRequest(error.Message)
            }
        );
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        var command = new CreateProductCommand
        {
            Name = request.Name,
            Price = request.Price
        };
        
        var result = await _mediator.Send(command);
        
        return result.Match(
            onSuccess: id => CreatedAtAction(nameof(GetProduct), new { id }, null),
            onFailure: error => BadRequest(error.Message)
        );
    }
}
```

### Domain Event Handling Pipeline

```csharp
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventDispatcher> _logger;
    
    public DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    public async Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            _logger.LogInformation("Dispatching domain event {EventType} with ID {EventId}", 
                domainEvent.GetType().Name, domainEvent.Id);
                
            await _mediator.Publish(domainEvent);
        }
    }
}

// Usage in a command handler
public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, Guid>
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    
    public async Task<Result<Guid>> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        // Create and place order
        var order = new Order(Guid.NewGuid());
        foreach (var item in command.Items)
        {
            order.AddOrderLine(item.ProductId, item.Quantity);
        }
        
        order.Place();
        
        // Save order and dispatch events
        var saveResult = await _orderRepository.SaveAsync(order);
        
        if (saveResult.IsSuccess)
        {
            await _eventDispatcher.DispatchEventsAsync(order.GetDomainEvents());
            order.ClearDomainEvents();
        }
        
        return saveResult;
    }
}
```

## Best Practices

### DDD Best Practices

1. **Use Value Objects** for concepts defined by their attributes (Money, Address, PhoneNumber)
2. **Create meaningful Aggregates** with clear boundaries and invariants
3. **Keep Entities focused** on domain behavior rather than data persistence
4. **Use strongly-typed IDs** to prevent mixing identifier types

### Functional Programming Best Practices

1. **Prefer `Result<T>` over exceptions** for expected error cases
2. **Use `Maybe<T>` instead of null** for optional values
3. **Chain operations with `Bind` and `Map`** instead of using nested conditionals
4. **Use `Match` for exhaustive handling** of all possible states

### Domain Markers Best Practices

1. **Combine Domain Markers** when an entity needs multiple cross-cutting concerns
2. **Centralize implementation logic** in repositories or middleware
3. **Consider performance impacts** of global filters for soft deletion and multi-tenancy
4. **Apply markers consistently** across related entities

### Messaging Best Practices

1. **Commands should represent intent** and map to a single use case
2. **Queries should be idempotent** and not modify state
3. **Use domain events for cross-aggregate communication**
4. **Include correlation IDs** for traceability across system boundaries

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the Apache License 2.0. See the LICENSE file for details.
