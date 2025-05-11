# CoreKernel.Primitives Library

The `CoreKernel.Primitives` library provides foundational building blocks for Domain-Driven Design (DDD) in C#. It includes abstractions for **entities**, **aggregate roots**, **value objects**, and **strongly-typed IDs**, empowering developers to model business domains with clarity and consistency.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Entities](#entities)
  - [Aggregate Roots](#aggregate-roots)
  - [Domain Events](#domain-events)
  - [Value Objects](#value-objects)
  - [Strongly-Typed IDs](#strongly-typed-ids)
- [Best Practices](#best-practices)

---

## Overview

The `CoreKernel.Primitives` project encapsulates essential DDD concepts as reusable abstractions. It enables consistent modeling of domain entities, immutability of value objects, and explicit handling of domain events. By enforcing these patterns early, it ensures better code maintainability and domain integrity.

---

## Features

- **Entity Base Class**
  - Implements identity-based equality comparison.
  - Supports generic ID types and immutability by default.

- **Aggregate Root Base Class**
  - Extends `Entity<TId>` with domain event tracking.
  - Provides methods to raise, retrieve, and clear domain events.

- **Domain Event Marker Interface**
  - A lightweight interface for tagging domain events in the model layer.

- **Value Object Base Class**
  - Implements structural equality via atomic values.
  - Promotes immutability and encapsulation.

- **Strongly-Typed ID Base Class**
  - Prevents accidental type mismatches in identifiers.
  - Enforces type-safe ID value representation.

---

## Installation

Add the `CoreKernel.Primitives` package to your project:

```bash
dotnet add package CoreKernel.Primitives
```

---

## Usage

### Entities

The `Entity<TId>` base class provides equality comparison based on the identifier:

```csharp
public class Product : Entity<Guid>
{
    public string Name { get; }

    public Product(Guid id, string name) : base(id)
    {
        Name = name;
    }
}
```

Entities are compared by ID and type:

```csharp
var product1 = new Product(Guid.NewGuid(), "Widget");
var product2 = new Product(product1.Id, "Widget");

bool areEqual = product1 == product2; // true
```

---

### Aggregate Roots

Aggregate roots represent the entry point to a domain aggregate. They can raise domain events:

```csharp
public class Order : AggregateRoot<Guid>
{
    public Order(Guid id) : base(id) { }

    public void Place()
    {
        RaiseDomainEvent(new OrderPlaced(Id));
    }
}
```

Working with domain events:

```csharp
var order = new Order(Guid.NewGuid());
order.Place();

var events = order.GetDomainEvents(); // IReadOnlyCollection<IDomainEvent>
order.ClearDomainEvents(); // After publishing
```

---

### Domain Events

The `IDomainEvent` interface is a simple marker used to denote significant changes in the domain:

```csharp
public class OrderPlaced : IDomainEvent
{
    public Guid OrderId { get; }

    public OrderPlaced(Guid orderId)
    {
        OrderId = orderId;
    }
}
```

This pattern allows domain models to remain decoupled from event publishing concerns.

---

### Value Objects

Create value objects by inheriting from `ValueObject` and defining atomic values:

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
}
```

Value objects are compared by structure, not reference:

```csharp
var price1 = new Money(100, "USD");
var price2 = new Money(100, "USD");

bool areEqual = price1 == price2; // true
```

---

### Strongly-Typed IDs

Define a custom ID by inheriting from `StronglyTypedId<T>`:

```csharp
public class UserId : StronglyTypedId<Guid>
{
    public UserId(Guid value) : base(value) { }
}
```

Usage:

```csharp
var id1 = new UserId(Guid.NewGuid());
var id2 = new UserId(id1.Value);

bool areEqual = id1 == id2; // true (if same GUID)
```

This approach eliminates accidental ID misuse between different entity types.

---

## Best Practices

1. **Use Value Objects** for any data without identity (e.g., Address, Money, Range).
2. **Derive all domain entities from `Entity<TId>`** to ensure consistent equality logic.
3. **Only expose aggregate roots** in your repositories to maintain aggregate boundaries.
4. **Raise domain events inside aggregate methods** to maintain invariants.
5. **Leverage `StronglyTypedId<T>`** to prevent bugs caused by mixing ID types.
6. **Avoid setters in entities and value objects** to preserve immutability.