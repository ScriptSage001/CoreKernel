using CoreKernel.DomainMarkers.Auditing;
using CoreKernel.DomainMarkers.MultiTenancy;
using CoreKernel.DomainMarkers.SoftDeletion;
using CoreKernel.DomainMarkers.Tracing;
using FluentAssertions;

namespace CoreKernel.DomainMarkers.Tests;

/// <summary>
/// Contains unit tests that verify correct implementation of domain marker interfaces.
/// These tests use sample entity implementations to ensure the interfaces work as expected.
/// </summary>
public class DomainMarkerImplementationTests
{
    #region Test Entity Classes

    /// <summary>
    /// A test entity that implements <see cref="IAuditable"/>.
    /// </summary>
    private class AuditableEntity : IAuditable
    {
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    /// <summary>
    /// A test entity that implements <see cref="ITimeStamped"/>.
    /// </summary>
    private class TimeStampedEntity : ITimeStamped
    {
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
    }

    /// <summary>
    /// A test entity that implements <see cref="ISoftDeletable"/>.
    /// </summary>
    private class SoftDeletableEntity : ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedOn { get; set; }
        public string? DeletedBy { get; set; }
    }

    /// <summary>
    /// A test entity that implements <see cref="ITenantScoped{TId}"/> with Guid.
    /// </summary>
    private class TenantScopedEntity : ITenantScoped<Guid>
    {
        public Guid TenantId { get; set; }
    }

    /// <summary>
    /// A test entity that implements <see cref="ITenantScoped{TId}"/> with string.
    /// </summary>
    private class StringTenantScopedEntity : ITenantScoped<string>
    {
        public string? TenantId { get; set; }
    }

    /// <summary>
    /// A test entity that implements <see cref="ITraceable"/>.
    /// </summary>
    private class TraceableEntity : ITraceable
    {
        public Guid CorrelationId { get; set; }
        public string? TraceSource { get; set; }
        public string? OperationName { get; set; }
    }

    /// <summary>
    /// A test entity that implements multiple domain markers.
    /// </summary>
    private class FullyMarkedEntity : IAuditable, ISoftDeletable, ITenantScoped<Guid>, ITraceable
    {
        // IAuditable / ITimeStamped
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

        // ISoftDeletable
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedOn { get; set; }
        public string? DeletedBy { get; set; }

        // ITenantScoped
        public Guid TenantId { get; set; }

        // ITraceable
        public Guid CorrelationId { get; set; }
        public string? TraceSource { get; set; }
        public string? OperationName { get; set; }
    }

    #endregion

    #region IAuditable Implementation Tests

    /// <summary>
    /// Verifies that an entity implementing <see cref="IAuditable"/> can set and get the CreatedBy property.
    /// </summary>
    [Fact]
    public void AuditableEntity_Should_SetAndGetCreatedBy()
    {
        // Arrange
        var entity = new AuditableEntity();
        var createdBy = "user@example.com";

        // Act
        entity.CreatedBy = createdBy;

        // Assert
        entity.CreatedBy.Should().Be(createdBy);
    }

    /// <summary>
    /// Verifies that an entity implementing <see cref="IAuditable"/> can set and get the LastModifiedBy property.
    /// </summary>
    [Fact]
    public void AuditableEntity_Should_SetAndGetLastModifiedBy()
    {
        // Arrange
        var entity = new AuditableEntity();
        var modifiedBy = "admin@example.com";

        // Act
        entity.LastModifiedBy = modifiedBy;

        // Assert
        entity.LastModifiedBy.Should().Be(modifiedBy);
    }

    /// <summary>
    /// Verifies that an entity implementing <see cref="IAuditable"/> inherits the timestamp properties.
    /// </summary>
    [Fact]
    public void AuditableEntity_Should_InheritTimeStampProperties()
    {
        // Arrange
        var entity = new AuditableEntity();
        var now = DateTimeOffset.UtcNow;

        // Act
        entity.CreatedOn = now;
        entity.LastModifiedOn = now.AddHours(1);

        // Assert
        entity.CreatedOn.Should().Be(now);
        entity.LastModifiedOn.Should().Be(now.AddHours(1));
    }

    #endregion

    #region ITimeStamped Implementation Tests

    /// <summary>
    /// Verifies that an entity implementing <see cref="ITimeStamped"/> can set and get the CreatedOn property.
    /// </summary>
    [Fact]
    public void TimeStampedEntity_Should_SetAndGetCreatedOn()
    {
        // Arrange
        var entity = new TimeStampedEntity();
        var createdOn = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero);

        // Act
        entity.CreatedOn = createdOn;

        // Assert
        entity.CreatedOn.Should().Be(createdOn);
    }

    /// <summary>
    /// Verifies that an entity implementing <see cref="ITimeStamped"/> can set and get the LastModifiedOn property.
    /// </summary>
    [Fact]
    public void TimeStampedEntity_Should_SetAndGetLastModifiedOn()
    {
        // Arrange
        var entity = new TimeStampedEntity();
        var modifiedOn = new DateTimeOffset(2024, 1, 16, 14, 45, 0, TimeSpan.Zero);

        // Act
        entity.LastModifiedOn = modifiedOn;

        // Assert
        entity.LastModifiedOn.Should().Be(modifiedOn);
    }

    /// <summary>
    /// Verifies that DateTimeOffset properties preserve timezone offset information.
    /// </summary>
    [Fact]
    public void TimeStampedEntity_Should_PreserveTimezoneOffset()
    {
        // Arrange
        var entity = new TimeStampedEntity();
        var createdOn = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(5));

        // Act
        entity.CreatedOn = createdOn;

        // Assert
        entity.CreatedOn.Offset.Should().Be(TimeSpan.FromHours(5));
    }

    #endregion

    #region ISoftDeletable Implementation Tests

    /// <summary>
    /// Verifies that an entity implementing <see cref="ISoftDeletable"/> can be marked as deleted.
    /// </summary>
    [Fact]
    public void SoftDeletableEntity_Should_SetAndGetIsDeleted()
    {
        // Arrange
        var entity = new SoftDeletableEntity();

        // Act
        entity.IsDeleted = true;

        // Assert
        entity.IsDeleted.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that an entity implementing <see cref="ISoftDeletable"/> tracks deletion time.
    /// </summary>
    [Fact]
    public void SoftDeletableEntity_Should_SetAndGetDeletedOn()
    {
        // Arrange
        var entity = new SoftDeletableEntity();
        var deletedOn = DateTimeOffset.UtcNow;

        // Act
        entity.DeletedOn = deletedOn;

        // Assert
        entity.DeletedOn.Should().Be(deletedOn);
    }

    /// <summary>
    /// Verifies that an entity implementing <see cref="ISoftDeletable"/> tracks who deleted it.
    /// </summary>
    [Fact]
    public void SoftDeletableEntity_Should_SetAndGetDeletedBy()
    {
        // Arrange
        var entity = new SoftDeletableEntity();
        var deletedBy = "admin@example.com";

        // Act
        entity.DeletedBy = deletedBy;

        // Assert
        entity.DeletedBy.Should().Be(deletedBy);
    }

    /// <summary>
    /// Verifies the complete soft deletion workflow.
    /// </summary>
    [Fact]
    public void SoftDeletableEntity_Should_SupportFullDeletionWorkflow()
    {
        // Arrange
        var entity = new SoftDeletableEntity();
        var deletedBy = "system";
        var deletedOn = DateTimeOffset.UtcNow;

        // Act - Simulate soft delete
        entity.IsDeleted = true;
        entity.DeletedOn = deletedOn;
        entity.DeletedBy = deletedBy;

        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedOn.Should().Be(deletedOn);
        entity.DeletedBy.Should().Be(deletedBy);
    }

    /// <summary>
    /// Verifies that DeletedOn can be null when entity has not been deleted.
    /// </summary>
    [Fact]
    public void SoftDeletableEntity_DeletedOn_Should_BeNullableWhenNotDeleted()
    {
        // Arrange
        var entity = new SoftDeletableEntity();

        // Assert
        entity.DeletedOn.Should().BeNull();
        entity.IsDeleted.Should().BeFalse();
    }

    #endregion

    #region ITenantScoped Implementation Tests

    /// <summary>
    /// Verifies that an entity implementing <see cref="ITenantScoped{TId}"/> can be assigned a tenant.
    /// </summary>
    [Fact]
    public void TenantScopedEntity_Should_SetAndGetTenantId()
    {
        // Arrange
        var entity = new TenantScopedEntity();
        var tenantId = Guid.NewGuid();

        // Act
        entity.TenantId = tenantId;

        // Assert
        entity.TenantId.Should().Be(tenantId);
    }

    /// <summary>
    /// Verifies that <see cref="ITenantScoped{TId}"/> works with string IDs.
    /// </summary>
    [Fact]
    public void TenantScopedEntity_WithStringId_Should_SetAndGetTenantId()
    {
        // Arrange
        var entity = new StringTenantScopedEntity();
        var tenantId = "tenant-001";

        // Act
        entity.TenantId = tenantId;

        // Assert
        entity.TenantId.Should().Be(tenantId);
    }

    #endregion

    #region ITraceable Implementation Tests

    /// <summary>
    /// Verifies that an entity implementing <see cref="ITraceable"/> can store correlation ID.
    /// </summary>
    [Fact]
    public void TraceableEntity_Should_SetAndGetCorrelationId()
    {
        // Arrange
        var entity = new TraceableEntity();
        var correlationId = Guid.NewGuid();

        // Act
        entity.CorrelationId = correlationId;

        // Assert
        entity.CorrelationId.Should().Be(correlationId);
    }

    /// <summary>
    /// Verifies that an entity implementing <see cref="ITraceable"/> can store trace source.
    /// </summary>
    [Fact]
    public void TraceableEntity_Should_SetAndGetTraceSource()
    {
        // Arrange
        var entity = new TraceableEntity();
        var traceSource = "OrderService";

        // Act
        entity.TraceSource = traceSource;

        // Assert
        entity.TraceSource.Should().Be(traceSource);
    }

    /// <summary>
    /// Verifies that an entity implementing <see cref="ITraceable"/> can store operation name.
    /// </summary>
    [Fact]
    public void TraceableEntity_Should_SetAndGetOperationName()
    {
        // Arrange
        var entity = new TraceableEntity();
        var operationName = "CreateOrder";

        // Act
        entity.OperationName = operationName;

        // Assert
        entity.OperationName.Should().Be(operationName);
    }

    #endregion

    #region Multiple Markers Implementation Tests

    /// <summary>
    /// Verifies that an entity can implement multiple domain markers simultaneously.
    /// </summary>
    [Fact]
    public void Entity_Should_SupportMultipleMarkers()
    {
        // Arrange
        var entity = new FullyMarkedEntity();
        var now = DateTimeOffset.UtcNow;
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        // Act
        entity.CreatedOn = now;
        entity.CreatedBy = "user@example.com";
        entity.TenantId = tenantId;
        entity.CorrelationId = correlationId;
        entity.TraceSource = "TestService";
        entity.OperationName = "CreateEntity";

        // Assert
        entity.Should().BeAssignableTo<IAuditable>();
        entity.Should().BeAssignableTo<ISoftDeletable>();
        entity.Should().BeAssignableTo<ITenantScoped<Guid>>();
        entity.Should().BeAssignableTo<ITraceable>();
        
        entity.CreatedOn.Should().Be(now);
        entity.CreatedBy.Should().Be("user@example.com");
        entity.TenantId.Should().Be(tenantId);
        entity.CorrelationId.Should().Be(correlationId);
    }

    /// <summary>
    /// Verifies that an entity implementing multiple markers can be soft deleted while retaining audit info.
    /// </summary>
    [Fact]
    public void FullyMarkedEntity_Should_SupportSoftDeleteWithAuditTrail()
    {
        // Arrange
        var entity = new FullyMarkedEntity
        {
            CreatedOn = DateTimeOffset.UtcNow.AddDays(-1),
            CreatedBy = "creator@example.com",
            TenantId = Guid.NewGuid()
        };

        // Act - Soft delete
        entity.IsDeleted = true;
        entity.DeletedOn = DateTimeOffset.UtcNow;
        entity.DeletedBy = "admin@example.com";

        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedBy.Should().Be("admin@example.com");
        entity.CreatedBy.Should().Be("creator@example.com"); // Audit info retained
    }

    #endregion
}
