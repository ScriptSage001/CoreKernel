# CoreKernel.DomainMarkers Library

The `CoreKernel.DomainMarkers` library provides a set of interfaces that serve as markers for common cross-cutting concerns in domain entities. These markers help standardize entity behaviors across your application while enabling automatic handling of concerns like auditing, multi-tenancy, soft deletion, and distributed tracing.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Auditing](#auditing)
  - [Multi-Tenancy](#multi-tenancy)
  - [Soft Deletion](#soft-deletion)
  - [Tracing](#tracing)
- [Integration Examples](#integration-examples)
  - [Repository Implementations](#repository-implementations)
  - [Middleware and Filters](#middleware-and-filters)
  - [Database Filters](#database-filters)
- [Best Practices](#best-practices)

---

## Overview

The `CoreKernel.DomainMarkers` project encapsulates essential cross-cutting concerns as reusable marker interfaces. It enables consistent modeling of entity behaviors across an application, promoting maintainability and reducing boilerplate code. These markers can be combined to implement multiple aspects in a single entity.

---

## Features

- **Auditing**
  - Track when entities are created and modified
  - Record which users performed entity changes

- **Multi-Tenancy**
  - Scope entities to specific tenants in multi-tenant applications
  - Type-safe tenant identifier tracking

- **Soft Deletion**
  - Mark entities as deleted without physical removal
  - Track when and by whom entities were deleted

- **Tracing**
  - Maintain correlation IDs across distributed systems
  - Record operation sources and names for troubleshooting

---

## Installation

Add the `CoreKernel.DomainMarkers` package to your project:

```bash
dotnet add package CoreKernel.DomainMarkers
```

---

## Usage

### Auditing

The auditing interfaces provide mechanisms to track entity creation and modification:

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

---

### Multi-Tenancy

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

This allows for automatic tenant filtering in queries and prevents cross-tenant data access.

---

### Soft Deletion

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
    
    public void Delete(string userId)
    {
        IsDeleted = true;
        DeletedOn = DateTimeOffset.UtcNow;
        DeletedBy = userId;
    }
}
```

---

### Tracing

The `ITraceable` interface enables distributed tracing across systems:

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

---

## Integration Examples

### Repository Implementations

Domain markers can be leveraged in generic repository implementations:

```csharp
public class GenericRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    
    public GenericRepository(DbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }
    
    public async Task SaveAsync(TEntity entity)
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
        else if (entity is ITimeStamped timeStamped)
        {
            var now = DateTimeOffset.UtcNow;
            
            if (IsNew(entity))
            {
                timeStamped.CreatedOn = now;
            }
            
            timeStamped.LastModifiedOn = now;
        }
        
        // Save entity logic...
        await _dbContext.SaveChangesAsync();
    }
    
    private bool IsNew(TEntity entity)
    {
        return !_dbContext.Entry(entity).IsKeySet;
    }
}
```

### Middleware and Filters

Domain markers can be utilized in middleware or filters for automatic handling:

```csharp
public class AuditingMiddleware<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    
    public AuditingMiddleware(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Apply auditing to command handlers that modify IAuditable entities
        if (request is IEntityCommand command && command.Entity is IAuditable auditable)
        {
            var now = DateTimeOffset.UtcNow;
            var userId = _currentUserService.UserId ?? "system";
            
            if (command.IsNew)
            {
                auditable.CreatedOn = now;
                auditable.CreatedBy = userId;
            }
            
            auditable.LastModifiedOn = now;
            auditable.LastModifiedBy = userId;
        }
        
        return await next();
    }
}
```

### Database Filters

Entity Framework Core global query filters can leverage the domain markers:

```csharp
public class ApplicationDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    
    public ApplicationDbContext(DbContextOptions options, ITenantProvider tenantProvider) 
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply soft delete filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var expression = Expression.Equal(property, falseConstant);
                var lambda = Expression.Lambda(expression, parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
            
            // Apply multi-tenancy filter for Guid tenant IDs
            if (typeof(ITenantScoped<Guid>).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ITenantScoped<Guid>.TenantId));
                var tenantId = Expression.Constant(_tenantProvider.GetCurrentTenantId());
                var expression = Expression.Equal(property, tenantId);
                var lambda = Expression.Lambda(expression, parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
```

---

## Best Practices

1. **Combine Domain Markers** - Use multiple interfaces on an entity to address all relevant cross-cutting concerns.
2. **Centralize Implementation Logic** - Handle the marker interfaces in middleware, repositories, or database context rather than duplicating logic.
3. **Consider Performance** - Be mindful of how filters affect query performance, especially with complex combinations of markers.
4. **Isolation of Concerns** - Use markers to maintain a clean separation between domain logic and cross-cutting concerns.
5. **Automatic Handling** - Implement automatic handling of domain markers in infrastructure layers to minimize boilerplate code.
6. **Consistent Application** - Apply the same markers consistently across related entities.
7. **Type Safety** - Leverage generic type parameters for tenant and other IDs to ensure type safety.
8. **Don't Mix Domain Logic** - Keep the marker interfaces focused on infrastructure concerns and separate from domain logic.
9. **Include in Base Classes** - Consider creating base entity classes that implement common combinations of markers.
10. **Documentation** - Clearly document which markers an entity implements to make the cross-cutting concerns explicit.
