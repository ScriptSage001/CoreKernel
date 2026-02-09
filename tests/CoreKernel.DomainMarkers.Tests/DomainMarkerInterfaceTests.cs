using CoreKernel.DomainMarkers.Auditing;
using CoreKernel.DomainMarkers.MultiTenancy;
using CoreKernel.DomainMarkers.SoftDeletion;
using CoreKernel.DomainMarkers.Tracing;
using FluentAssertions;

namespace CoreKernel.DomainMarkers.Tests;

/// <summary>
/// Contains unit tests that verify the structure and inheritance of domain marker interfaces.
/// These tests ensure that the interfaces define the correct properties and relationships.
/// </summary>
public class DomainMarkerInterfaceTests
{
    #region IAuditable Tests

    /// <summary>
    /// Verifies that <see cref="IAuditable"/> inherits from <see cref="ITimeStamped"/>.
    /// </summary>
    [Fact]
    public void IAuditable_Should_InheritFromITimeStamped()
    {
        // Assert
        typeof(IAuditable).Should().BeAssignableTo<ITimeStamped>();
    }

    /// <summary>
    /// Verifies that <see cref="IAuditable"/> defines the <see cref="IAuditable.CreatedBy"/> property.
    /// </summary>
    [Fact]
    public void IAuditable_Should_HaveCreatedByProperty()
    {
        // Assert
        var property = typeof(IAuditable).GetProperty(nameof(IAuditable.CreatedBy));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(string));
    }

    /// <summary>
    /// Verifies that <see cref="IAuditable"/> defines the <see cref="IAuditable.LastModifiedBy"/> property.
    /// </summary>
    [Fact]
    public void IAuditable_Should_HaveLastModifiedByProperty()
    {
        // Assert
        var property = typeof(IAuditable).GetProperty(nameof(IAuditable.LastModifiedBy));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(string));
    }

    #endregion

    #region ITimeStamped Tests

    /// <summary>
    /// Verifies that <see cref="ITimeStamped"/> defines the <see cref="ITimeStamped.CreatedOn"/> property with DateTimeOffset type.
    /// </summary>
    [Fact]
    public void ITimeStamped_Should_HaveCreatedOnProperty()
    {
        // Assert
        var property = typeof(ITimeStamped).GetProperty(nameof(ITimeStamped.CreatedOn));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(DateTimeOffset));
    }

    /// <summary>
    /// Verifies that <see cref="ITimeStamped"/> defines the <see cref="ITimeStamped.LastModifiedOn"/> property with DateTimeOffset type.
    /// </summary>
    [Fact]
    public void ITimeStamped_Should_HaveLastModifiedOnProperty()
    {
        // Assert
        var property = typeof(ITimeStamped).GetProperty(nameof(ITimeStamped.LastModifiedOn));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(DateTimeOffset));
    }

    #endregion

    #region ISoftDeletable Tests

    /// <summary>
    /// Verifies that <see cref="ISoftDeletable"/> defines the <see cref="ISoftDeletable.IsDeleted"/> property.
    /// </summary>
    [Fact]
    public void ISoftDeletable_Should_HaveIsDeletedProperty()
    {
        // Assert
        var property = typeof(ISoftDeletable).GetProperty(nameof(ISoftDeletable.IsDeleted));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(bool));
    }

    /// <summary>
    /// Verifies that <see cref="ISoftDeletable"/> defines the <see cref="ISoftDeletable.DeletedOn"/> property with nullable DateTimeOffset type.
    /// </summary>
    [Fact]
    public void ISoftDeletable_Should_HaveDeletedOnProperty()
    {
        // Assert
        var property = typeof(ISoftDeletable).GetProperty(nameof(ISoftDeletable.DeletedOn));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(DateTimeOffset?));
    }

    /// <summary>
    /// Verifies that <see cref="ISoftDeletable"/> defines the <see cref="ISoftDeletable.DeletedBy"/> property.
    /// </summary>
    [Fact]
    public void ISoftDeletable_Should_HaveDeletedByProperty()
    {
        // Assert
        var property = typeof(ISoftDeletable).GetProperty(nameof(ISoftDeletable.DeletedBy));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(string));
    }

    #endregion

    #region ITenantScoped Tests

    /// <summary>
    /// Verifies that <see cref="ITenantScoped{TId}"/> defines the TenantId property.
    /// </summary>
    [Fact]
    public void ITenantScoped_Should_HaveTenantIdProperty()
    {
        // Assert
        var property = typeof(ITenantScoped<Guid>).GetProperty("TenantId");
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(Guid));
    }

    /// <summary>
    /// Verifies that <see cref="ITenantScoped{TId}"/> is generic and supports different ID types.
    /// </summary>
    [Fact]
    public void ITenantScoped_Should_SupportDifferentIdTypes()
    {
        // Assert
        typeof(ITenantScoped<int>).GetProperty("TenantId")!.PropertyType.Should().Be(typeof(int));
        typeof(ITenantScoped<string>).GetProperty("TenantId")!.PropertyType.Should().Be(typeof(string));
        typeof(ITenantScoped<long>).GetProperty("TenantId")!.PropertyType.Should().Be(typeof(long));
    }

    #endregion

    #region ITraceable Tests

    /// <summary>
    /// Verifies that <see cref="ITraceable"/> defines the <see cref="ITraceable.CorrelationId"/> property with Guid type.
    /// </summary>
    [Fact]
    public void ITraceable_Should_HaveCorrelationIdProperty()
    {
        // Assert
        var property = typeof(ITraceable).GetProperty(nameof(ITraceable.CorrelationId));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(Guid));
    }

    /// <summary>
    /// Verifies that <see cref="ITraceable"/> defines the <see cref="ITraceable.TraceSource"/> property.
    /// </summary>
    [Fact]
    public void ITraceable_Should_HaveTraceSourceProperty()
    {
        // Assert
        var property = typeof(ITraceable).GetProperty(nameof(ITraceable.TraceSource));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(string));
    }

    /// <summary>
    /// Verifies that <see cref="ITraceable"/> defines the <see cref="ITraceable.OperationName"/> property.
    /// </summary>
    [Fact]
    public void ITraceable_Should_HaveOperationNameProperty()
    {
        // Assert
        var property = typeof(ITraceable).GetProperty(nameof(ITraceable.OperationName));
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(string));
    }

    #endregion

    #region Interface Property Count Tests

    /// <summary>
    /// Verifies that <see cref="ITimeStamped"/> has the expected number of properties.
    /// </summary>
    [Fact]
    public void ITimeStamped_Should_HaveExpectedPropertyCount()
    {
        // Assert
        typeof(ITimeStamped).GetProperties().Should().HaveCount(2);
    }

    /// <summary>
    /// Verifies that <see cref="ISoftDeletable"/> has the expected number of properties.
    /// </summary>
    [Fact]
    public void ISoftDeletable_Should_HaveExpectedPropertyCount()
    {
        // Assert
        typeof(ISoftDeletable).GetProperties().Should().HaveCount(3);
    }

    /// <summary>
    /// Verifies that <see cref="ITraceable"/> has the expected number of properties.
    /// </summary>
    [Fact]
    public void ITraceable_Should_HaveExpectedPropertyCount()
    {
        // Assert
        typeof(ITraceable).GetProperties().Should().HaveCount(3);
    }

    #endregion
}
