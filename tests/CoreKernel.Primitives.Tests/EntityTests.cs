using CoreKernel.Primitives.Entities;
using FluentAssertions;

namespace CoreKernel.Primitives.Tests;

/// <summary>
/// Contains unit tests for the <see cref="Entity{TId}"/> abstract class.
/// These tests verify identity-based equality, hash code generation, and operator overloads.
/// </summary>
public class EntityTests
{
    #region Test Entities

    /// <summary>
    /// A concrete implementation of <see cref="Entity{TId}"/> for testing purposes.
    /// </summary>
    private class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id) { }
        
        public string? Name { get; set; }
    }

    /// <summary>
    /// A different entity type for testing cross-type comparisons.
    /// </summary>
    private class AnotherTestEntity : Entity<Guid>
    {
        public AnotherTestEntity(Guid id) : base(id) { }
    }

    #endregion

    #region Equality Tests

    /// <summary>
    /// Verifies that two entities with the same ID and type are considered equal.
    /// </summary>
    [Fact]
    public void Equals_WithSameIdAndType_Should_ReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        entity1.Equals(entity2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that two entities with different IDs are not considered equal.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentIds_Should_ReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that comparing an entity with null returns false.
    /// </summary>
    [Fact]
    public void Equals_WithNull_Should_ReturnFalse()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());

        // Act & Assert
        entity.Equals(null).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that two entities of different types with the same ID are not considered equal.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentType_Should_ReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new AnotherTestEntity(id);

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that the object.Equals method works correctly for entities.
    /// </summary>
    [Fact]
    public void ObjectEquals_WithSameEntity_Should_ReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        object entity2 = new TestEntity(id);

        // Act & Assert
        entity1.Equals(entity2).Should().BeTrue();
    }

    #endregion

    #region Operator Tests

    /// <summary>
    /// Verifies that the equality operator returns true for entities with the same ID.
    /// </summary>
    [Fact]
    public void EqualityOperator_WithSameId_Should_ReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that the equality operator returns false for entities with different IDs.
    /// </summary>
    [Fact]
    public void EqualityOperator_WithDifferentIds_Should_ReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        (entity1 == entity2).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that the equality operator returns false when comparing with null.
    /// </summary>
    [Fact]
    public void EqualityOperator_WithNull_Should_ReturnFalse()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());

        // Act & Assert
        (entity == null).Should().BeFalse();
        (null == entity).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that the inequality operator returns true for entities with different IDs.
    /// </summary>
    [Fact]
    public void InequalityOperator_WithDifferentIds_Should_ReturnTrue()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        (entity1 != entity2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that the inequality operator returns false for entities with the same ID.
    /// </summary>
    [Fact]
    public void InequalityOperator_WithSameId_Should_ReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        (entity1 != entity2).Should().BeFalse();
    }

    #endregion

    #region GetHashCode Tests

    /// <summary>
    /// Verifies that entities with the same ID produce the same hash code.
    /// </summary>
    [Fact]
    public void GetHashCode_WithSameId_Should_ReturnSameHashCode()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        entity1.GetHashCode().Should().Be(entity2.GetHashCode());
    }

    /// <summary>
    /// Verifies that entities with different IDs produce different hash codes (with high probability).
    /// </summary>
    [Fact]
    public void GetHashCode_WithDifferentIds_Should_ReturnDifferentHashCodes()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        entity1.GetHashCode().Should().NotBe(entity2.GetHashCode());
    }

    /// <summary>
    /// Verifies that hash code is consistent across multiple calls.
    /// </summary>
    [Fact]
    public void GetHashCode_Should_BeConsistent()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());

        // Act
        var hashCode1 = entity.GetHashCode();
        var hashCode2 = entity.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    #endregion

    #region Id Property Tests

    /// <summary>
    /// Verifies that the Id property returns the correct value set during construction.
    /// </summary>
    [Fact]
    public void Id_Should_ReturnValueSetInConstructor()
    {
        // Arrange
        var expectedId = Guid.NewGuid();

        // Act
        var entity = new TestEntity(expectedId);

        // Assert
        entity.Id.Should().Be(expectedId);
    }

    #endregion
}
