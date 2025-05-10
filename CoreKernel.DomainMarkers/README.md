### Domain Markers

The `CoreKernel.DomainMarkers` namespace provides a set of interfaces that serve as markers for common cross-cutting concerns in domain entities. These markers help standardize entity behaviors across your application.

#### Auditing
- `ITimeStamped`: Interface for entities that need creation and modification timestamps.
  ```csharp
  public class Product : Entity<Guid>, ITimeStamped
  {
      public DateTimeOffset CreatedOn { get; set; }
      public DateTimeOffset LastModifiedOn { get; set; }
      
      // Other properties...
  }
  ```

- `IAuditable`: Extends `ITimeStamped` to add user tracking for entity changes.
  ```csharp
  public class Document : Entity<Guid>, IAuditable
  {
      public DateTimeOffset CreatedOn { get; set; }
      public string CreatedBy { get; set; }
      public DateTimeOffset LastModifiedOn { get; set; }
      public string LastModifiedBy { get; set; }
      
      // Other properties...
  }
  ```

#### Multi-Tenancy
- `ITenantScoped<TId>`: Interface for entities that belong to a specific tenant in a multi-tenant application.
  ```csharp
  public class CustomerRecord : Entity<Guid>, ITenantScoped<Guid>
  {
      public Guid TenantId { get; set; }
      public string CustomerName { get; set; }
      
      // Other properties...
  }
  ```

#### Soft Deletion
- `ISoftDeletable`: Interface for entities that support soft deletion rather than physical removal from storage.
  ```csharp
  public class Invoice : Entity<Guid>, ISoftDeletable
  {
      public bool IsDeleted { get; set; }
      public DateTimeOffset? DeletedOn { get; set; }
      public string? DeletedBy { get; set; }
      
      // Other properties...
      
      public void Delete(string userId)
      {
          IsDeleted = true;
          DeletedOn = DateTimeOffset.UtcNow;
          DeletedBy = userId;
      }
  }
  ```

#### Tracing
- `ITraceable`: Interface for entities that need to maintain tracing information for distributed systems and troubleshooting.
  ```csharp
  public class Payment : Entity<Guid>, ITraceable
  {
      public Guid CorrelationId { get; set; }
      public string? TraceSource { get; set; }
      public string? OperationName { get; set; }
      
      // Other properties...
  }
  ```

### Use Cases for Domain Markers

#### Implementing Cross-Cutting Concerns
Domain markers can be combined to implement multiple cross-cutting concerns in a single entity:

```csharp
public class Order : AggregateRoot<Guid>, IAuditable, ITenantScoped<Guid>, ISoftDeletable, ITraceable
{
    // IAuditable implementation
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset LastModifiedOn { get; set; }
    public string LastModifiedBy { get; set; }
    
    // ITenantScoped implementation
    public Guid TenantId { get; set; }
    
    // ISoftDeletable implementation
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    
    // ITraceable implementation
    public Guid CorrelationId { get; set; }
    public string? TraceSource { get; set; }
    public string? OperationName { get; set; }
    
    // Order-specific properties
    public List<OrderLine> OrderLines { get; private set; } = new();
    public decimal TotalAmount => OrderLines.Sum(line => line.Amount);
    
    // Business methods...
}
```

#### Repository Implementations
Domain markers can be leveraged in generic repository implementations:

```csharp
public class GenericRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    public async Task SaveAsync(TEntity entity, string userId)
    {
        // Handle audit information
        if (entity is IAuditable auditable)
        {
            var now = DateTimeOffset.UtcNow;
            
            if (IsNew(entity))
            {
                auditable.CreatedOn = now;
                auditable.CreatedBy = userId;
            }
            
            auditable.LastModifiedOn = now;
            auditable.LastModifiedBy = userId;
        }
        
        // Save entity logic...
    }
    
    public async Task<IEnumerable<TEntity>> GetForTenantAsync<TTenantId>(TTenantId tenantId) 
        where TTenantId : notnull
    {
        if (typeof(ITenantScoped<TTenantId>).IsAssignableFrom(typeof(TEntity)))
        {
            // Filter by tenant ID when retrieving entities
            return await _dbContext.Set<TEntity>()
                .OfType<ITenantScoped<TTenantId>>()
                .Where(e => EF.Property<TTenantId>(e, "TenantId").Equals(tenantId))
                .Cast<TEntity>()
                .ToListAsync();
        }
        
        throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} is not tenant-scoped with ID type {typeof(TTenantId).Name}");
    }
    
    // Other repository methods...
}
```

#### Middleware and Filters
Domain markers can be utilized in middleware or filters to automatically handle cross-cutting concerns:

```csharp
public class AuditingMiddleware<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    
    public AuditingMiddleware(ICurrentUserService currentUserService, IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Apply auditing to command handlers that modify IAuditable entities
        if (request is ICommand command && command.Entity is IAuditable auditable)
        {
            var now = _dateTime.Now;
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

#### Database Filters for Soft Delete
Entity Framework Core global query filters can leverage the `ISoftDeletable` marker:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Apply soft delete filter to all entities implementing ISoftDeletable
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
    }
}
```

#### Distributed Tracing
The `ITraceable` interface can be used to implement distributed tracing across microservices:

```csharp
public class TracingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ITraceContextAccessor _traceContextAccessor;
    
    public TracingBehavior(ITraceContextAccessor traceContextAccessor)
    {
        _traceContextAccessor = traceContextAccessor;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is ICommand command && command.Entity is ITraceable traceable)
        {
            traceable.CorrelationId = _traceContextAccessor.CorrelationId;
            traceable.TraceSource = _traceContextAccessor.Source;
            traceable.OperationName = typeof(TRequest).Name;
        }
        
        return await next();
    }
}
```

These domain markers provide a standardized approach to implementing common patterns across your domain model, promoting consistency and reducing boilerplate code.