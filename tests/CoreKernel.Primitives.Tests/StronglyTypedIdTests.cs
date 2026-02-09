using CoreKernel.Primitives.ValueObjects;
using FluentAssertions;

namespace CoreKernel.Primitives.Tests;

/// <summary>
/// Contains unit tests for the <see cref="StronglyTypedId{T}"/> abstract class.
/// These tests verify type-safe ID behavior, equality, and value storage.
/// </summary>
public class StronglyTypedIdTests
{
    #region Test Strongly Typed IDs

    /// <summary>
    /// A strongly typed ID for User entities.
    /// </summary>
    private class UserId : StronglyTypedId<Guid>
    {
        public UserId(Guid value) : base(value) { }
    }

    /// <summary>
    /// A strongly typed ID for Order entities.
    /// </summary>
    private class OrderId : StronglyTypedId<Guid>
    {
        public OrderId(Guid value) : base(value) { }
    }

    /// <summary>
    /// A strongly typed ID using string as the underlying type.
    /// </summary>
    private class StringId : StronglyTypedId<string>
    {
        public StringId(string value) : base(value) { }
    }

    /// <summary>
    /// A strongly typed ID using int as the underlying type.
    /// </summary>
    private class IntegerId : StronglyTypedId<int>
    {
        public IntegerId(int value) : base(value) { }
    }

    #endregion

    #region Value Storage Tests

    /// <summary>
    /// Verifies that the <see cref="StronglyTypedId{T}.Value"/> property correctly stores and returns the value.
    /// </summary>
    [Fact]
    public void Value_Should_ReturnStoredValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var userId = new UserId(guid);

        // Assert
        userId.Value.Should().Be(guid);
    }

    /// <summary>
    /// Verifies that string-based IDs store values correctly.
    /// </summary>
    [Fact]
    public void Value_WithStringType_Should_ReturnStoredValue()
    {
        // Arrange
        var value = "user-123";

        // Act
        var stringId = new StringId(value);

        // Assert
        stringId.Value.Should().Be(value);
    }

    /// <summary>
    /// Verifies that integer-based IDs store values correctly.
    /// </summary>
    [Fact]
    public void Value_WithIntType_Should_ReturnStoredValue()
    {
        // Arrange
        var value = 42;

        // Act
        var integerId = new IntegerId(value);

        // Assert
        integerId.Value.Should().Be(value);
    }

    #endregion

    #region Equality Tests

    /// <summary>
    /// Verifies that two strongly typed IDs with the same value and type are considered equal.
    /// </summary>
    [Fact]
    public void Equals_WithSameValueAndType_Should_ReturnTrue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId1 = new UserId(guid);
        var userId2 = new UserId(guid);

        // Act & Assert
        userId1.Equals(userId2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that two strongly typed IDs with different values are not considered equal.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentValues_Should_ReturnFalse()
    {
        // Arrange
        var userId1 = new UserId(Guid.NewGuid());
        var userId2 = new UserId(Guid.NewGuid());

        // Act & Assert
        userId1.Equals(userId2).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that two different ID types with the same underlying value are not considered equal.
    /// This is the key type-safety feature of strongly typed IDs.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentTypeSameValue_Should_ReturnFalse()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);
        var orderId = new OrderId(guid);

        // Act & Assert
        userId.Equals(orderId).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that comparing a strongly typed ID with null returns false.
    /// </summary>
    [Fact]
    public void Equals_WithNull_Should_ReturnFalse()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        // Act & Assert
        userId.Equals(null).Should().BeFalse();
    }

    #endregion

    #region Operator Tests

    /// <summary>
    /// Verifies that the equality operator returns true for IDs with the same value and type.
    /// </summary>
    [Fact]
    public void EqualityOperator_WithSameValueAndType_Should_ReturnTrue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId1 = new UserId(guid);
        var userId2 = new UserId(guid);

        // Act & Assert
        (userId1 == userId2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that the equality operator returns false for IDs with different values.
    /// </summary>
    [Fact]
    public void EqualityOperator_WithDifferentValues_Should_ReturnFalse()
    {
        // Arrange
        var userId1 = new UserId(Guid.NewGuid());
        var userId2 = new UserId(Guid.NewGuid());

        // Act & Assert
        (userId1 == userId2).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that the equality operator handles null comparisons correctly.
    /// </summary>
    [Fact]
    public void EqualityOperator_WithNull_Should_ReturnFalse()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        // Act & Assert
        (userId == null).Should().BeFalse();
        (null == userId).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that the inequality operator returns true for IDs with different values.
    /// </summary>
    [Fact]
    public void InequalityOperator_WithDifferentValues_Should_ReturnTrue()
    {
        // Arrange
        var userId1 = new UserId(Guid.NewGuid());
        var userId2 = new UserId(Guid.NewGuid());

        // Act & Assert
        (userId1 != userId2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that the inequality operator returns false for IDs with the same value.
    /// </summary>
    [Fact]
    public void InequalityOperator_WithSameValue_Should_ReturnFalse()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId1 = new UserId(guid);
        var userId2 = new UserId(guid);

        // Act & Assert
        (userId1 != userId2).Should().BeFalse();
    }

    #endregion

    #region GetHashCode Tests

    /// <summary>
    /// Verifies that IDs with the same value produce the same hash code.
    /// </summary>
    [Fact]
    public void GetHashCode_WithSameValue_Should_ReturnSameHashCode()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId1 = new UserId(guid);
        var userId2 = new UserId(guid);

        // Act & Assert
        userId1.GetHashCode().Should().Be(userId2.GetHashCode());
    }

    /// <summary>
    /// Verifies that IDs with different values produce different hash codes.
    /// </summary>
    [Fact]
    public void GetHashCode_WithDifferentValues_Should_ReturnDifferentHashCodes()
    {
        // Arrange
        var userId1 = new UserId(Guid.NewGuid());
        var userId2 = new UserId(Guid.NewGuid());

        // Act & Assert
        userId1.GetHashCode().Should().NotBe(userId2.GetHashCode());
    }

    /// <summary>
    /// Verifies that hash code is consistent across multiple calls.
    /// </summary>
    [Fact]
    public void GetHashCode_Should_BeConsistent()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        // Act
        var hashCode1 = userId.GetHashCode();
        var hashCode2 = userId.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    #endregion

    #region ToString Tests

    /// <summary>
    /// Verifies that ToString returns the string representation of the underlying value.
    /// </summary>
    [Fact]
    public void ToString_Should_ReturnValueAsString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        // Act
        var result = userId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    /// <summary>
    /// Verifies that ToString works correctly for string-based IDs.
    /// </summary>
    [Fact]
    public void ToString_WithStringValue_Should_ReturnValue()
    {
        // Arrange
        var value = "customer-456";
        var stringId = new StringId(value);

        // Act
        var result = stringId.ToString();

        // Assert
        result.Should().Be(value);
    }

    #endregion

    #region Type Safety Tests

    /// <summary>
    /// Verifies that strongly typed IDs prevent mixing different ID types at compile time.
    /// This test demonstrates the type safety benefit.
    /// </summary>
    [Fact]
    public void StronglyTypedId_Should_PreventTypeMixing()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);
        var orderId = new OrderId(guid);

        // Assert - Even with same underlying value, they are different types
        userId.Value.Should().Be(orderId.Value);
        userId.Equals(orderId).Should().BeFalse();
        userId.GetType().Should().NotBe(orderId.GetType());
    }

    /// <summary>
    /// Verifies that strongly typed IDs work correctly in collections.
    /// </summary>
    [Fact]
    public void StronglyTypedId_Should_WorkInDictionary()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId1 = new UserId(guid);
        var userId2 = new UserId(guid);
        var dictionary = new Dictionary<UserId, string>();

        // Act
        dictionary[userId1] = "John Doe";

        // Assert
        dictionary[userId2].Should().Be("John Doe");
    }

    /// <summary>
    /// Verifies that strongly typed IDs work correctly in HashSets.
    /// </summary>
    [Fact]
    public void StronglyTypedId_Should_WorkInHashSet()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId1 = new UserId(guid);
        var userId2 = new UserId(guid);
        var hashSet = new HashSet<UserId>();

        // Act
        hashSet.Add(userId1);
        hashSet.Add(userId2);

        // Assert
        hashSet.Should().ContainSingle();
    }

    #endregion
}
