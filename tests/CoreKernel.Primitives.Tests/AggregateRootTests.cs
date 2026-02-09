using CoreKernel.Primitives.Abstractions;
using CoreKernel.Primitives.Entities;
using FluentAssertions;

namespace CoreKernel.Primitives.Tests;

/// <summary>
/// Contains unit tests for the <see cref="AggregateRoot{TId}"/> abstract class.
/// These tests verify domain event management functionality including raising, retrieving, and clearing events.
/// </summary>
public class AggregateRootTests
{
    #region Test Classes

    /// <summary>
    /// A concrete implementation of <see cref="AggregateRoot{TId}"/> for testing purposes.
    /// </summary>
    private class TestAggregateRoot : AggregateRoot<Guid>
    {
        public TestAggregateRoot(Guid id) : base(id) { }

        /// <summary>
        /// Exposes the protected RaiseDomainEvent method for testing.
        /// </summary>
        public void RaiseTestEvent(IDomainEvent domainEvent) => RaiseDomainEvent(domainEvent);
    }

    /// <summary>
    /// A test domain event for testing purposes.
    /// </summary>
    private class TestDomainEvent : IDomainEvent
    {
        public string Message { get; }

        public TestDomainEvent(string message)
        {
            Message = message;
        }
    }

    /// <summary>
    /// Another test domain event for testing multiple event types.
    /// </summary>
    private class AnotherDomainEvent : IDomainEvent
    {
        public int Value { get; }

        public AnotherDomainEvent(int value)
        {
            Value = value;
        }
    }

    #endregion

    #region RaiseDomainEvent Tests

    /// <summary>
    /// Verifies that <see cref="AggregateRoot{TId}.RaiseDomainEvent"/> adds an event to the domain events collection.
    /// </summary>
    [Fact]
    public void RaiseDomainEvent_Should_AddEventToCollection()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
        var domainEvent = new TestDomainEvent("Test message");

        // Act
        aggregateRoot.RaiseTestEvent(domainEvent);

        // Assert
        aggregateRoot.GetDomainEvents().Should().ContainSingle();
        aggregateRoot.GetDomainEvents().Should().Contain(domainEvent);
    }

    /// <summary>
    /// Verifies that multiple domain events can be raised and all are stored.
    /// </summary>
    [Fact]
    public void RaiseDomainEvent_MultipleTimes_Should_AddAllEvents()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
        var event1 = new TestDomainEvent("Event 1");
        var event2 = new TestDomainEvent("Event 2");
        var event3 = new AnotherDomainEvent(42);

        // Act
        aggregateRoot.RaiseTestEvent(event1);
        aggregateRoot.RaiseTestEvent(event2);
        aggregateRoot.RaiseTestEvent(event3);

        // Assert
        aggregateRoot.GetDomainEvents().Should().HaveCount(3);
        aggregateRoot.GetDomainEvents().Should().Contain(event1);
        aggregateRoot.GetDomainEvents().Should().Contain(event2);
        aggregateRoot.GetDomainEvents().Should().Contain(event3);
    }

    #endregion

    #region GetDomainEvents Tests

    /// <summary>
    /// Verifies that <see cref="AggregateRoot{TId}.GetDomainEvents"/> returns an empty collection when no events have been raised.
    /// </summary>
    [Fact]
    public void GetDomainEvents_WithNoEvents_Should_ReturnEmptyCollection()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

        // Act
        var events = aggregateRoot.GetDomainEvents();

        // Assert
        events.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that <see cref="AggregateRoot{TId}.GetDomainEvents"/> returns events in the order they were raised.
    /// </summary>
    [Fact]
    public void GetDomainEvents_Should_ReturnEventsInOrder()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
        var event1 = new TestDomainEvent("First");
        var event2 = new TestDomainEvent("Second");
        var event3 = new TestDomainEvent("Third");

        // Act
        aggregateRoot.RaiseTestEvent(event1);
        aggregateRoot.RaiseTestEvent(event2);
        aggregateRoot.RaiseTestEvent(event3);
        var events = aggregateRoot.GetDomainEvents().ToList();

        // Assert
        events[0].Should().Be(event1);
        events[1].Should().Be(event2);
        events[2].Should().Be(event3);
    }

    /// <summary>
    /// Verifies that <see cref="AggregateRoot{TId}.GetDomainEvents"/> returns a read-only collection.
    /// </summary>
    [Fact]
    public void GetDomainEvents_Should_ReturnReadOnlyCollection()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
        aggregateRoot.RaiseTestEvent(new TestDomainEvent("Test"));

        // Act
        var events = aggregateRoot.GetDomainEvents();

        // Assert
        events.Should().BeAssignableTo<IReadOnlyCollection<IDomainEvent>>();
    }

    #endregion

    #region ClearDomainEvents Tests

    /// <summary>
    /// Verifies that <see cref="AggregateRoot{TId}.ClearDomainEvents"/> removes all events from the collection.
    /// </summary>
    [Fact]
    public void ClearDomainEvents_Should_RemoveAllEvents()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
        aggregateRoot.RaiseTestEvent(new TestDomainEvent("Event 1"));
        aggregateRoot.RaiseTestEvent(new TestDomainEvent("Event 2"));

        // Act
        aggregateRoot.ClearDomainEvents();

        // Assert
        aggregateRoot.GetDomainEvents().Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that <see cref="AggregateRoot{TId}.ClearDomainEvents"/> does not throw when called on empty collection.
    /// </summary>
    [Fact]
    public void ClearDomainEvents_WithNoEvents_Should_NotThrow()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

        // Act
        var act = () => aggregateRoot.ClearDomainEvents();

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Verifies that new events can be raised after clearing.
    /// </summary>
    [Fact]
    public void RaiseDomainEvent_AfterClear_Should_AddNewEvents()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
        aggregateRoot.RaiseTestEvent(new TestDomainEvent("Old event"));
        aggregateRoot.ClearDomainEvents();
        var newEvent = new TestDomainEvent("New event");

        // Act
        aggregateRoot.RaiseTestEvent(newEvent);

        // Assert
        aggregateRoot.GetDomainEvents().Should().ContainSingle();
        aggregateRoot.GetDomainEvents().Should().Contain(newEvent);
    }

    #endregion

    #region Inheritance Tests

    /// <summary>
    /// Verifies that <see cref="AggregateRoot{TId}"/> inherits from <see cref="Entity{TId}"/>.
    /// </summary>
    [Fact]
    public void AggregateRoot_Should_InheritFromEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var aggregateRoot = new TestAggregateRoot(id);

        // Assert
        aggregateRoot.Should().BeAssignableTo<Entity<Guid>>();
        aggregateRoot.Id.Should().Be(id);
    }

    /// <summary>
    /// Verifies that aggregate roots maintain entity equality semantics.
    /// </summary>
    [Fact]
    public void AggregateRoot_Equality_Should_BeBasedOnId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var aggregateRoot1 = new TestAggregateRoot(id);
        var aggregateRoot2 = new TestAggregateRoot(id);

        // Assert
        aggregateRoot1.Equals(aggregateRoot2).Should().BeTrue();
        (aggregateRoot1 == aggregateRoot2).Should().BeTrue();
    }

    #endregion
}
