using CoreKernel.Primitives.ValueObjects;
using FluentAssertions;

namespace CoreKernel.Primitives.Tests;

/// <summary>
/// Contains unit tests for the <see cref="ValueObject"/> abstract class.
/// These tests verify structural equality based on atomic values.
/// </summary>
public class ValueObjectTests
{
    #region Test Value Objects

    /// <summary>
    /// A concrete implementation of <see cref="ValueObject"/> representing an address.
    /// </summary>
    private class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string ZipCode { get; }

        public Address(string street, string city, string zipCode)
        {
            Street = street;
            City = city;
            ZipCode = zipCode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Street;
            yield return City;
            yield return ZipCode;
        }
    }

    /// <summary>
    /// A concrete implementation of <see cref="ValueObject"/> representing money.
    /// </summary>
    private class Money : ValueObject
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

    /// <summary>
    /// A value object with a single value for testing edge cases.
    /// </summary>
    private class SingleValue : ValueObject
    {
        public string Value { get; }

        public SingleValue(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }

    #endregion

    #region Equality Tests

    /// <summary>
    /// Verifies that two value objects with the same atomic values are considered equal.
    /// </summary>
    [Fact]
    public void Equals_WithSameValues_Should_ReturnTrue()
    {
        // Arrange
        var address1 = new Address("123 Main St", "New York", "10001");
        var address2 = new Address("123 Main St", "New York", "10001");

        // Act & Assert
        address1.Equals(address2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that two value objects with different atomic values are not considered equal.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentValues_Should_ReturnFalse()
    {
        // Arrange
        var address1 = new Address("123 Main St", "New York", "10001");
        var address2 = new Address("456 Oak Ave", "New York", "10001");

        // Act & Assert
        address1.Equals(address2).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that comparing a value object with null returns false.
    /// </summary>
    [Fact]
    public void Equals_WithNull_Should_ReturnFalse()
    {
        // Arrange
        var address = new Address("123 Main St", "New York", "10001");

        // Act & Assert
        address.Equals(null).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that two different types of value objects are not considered equal even with similar values.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentType_Should_ReturnFalse()
    {
        // Arrange
        var address = new Address("100", "USD", "10001");
        var money = new Money(100, "USD");

        // Act & Assert
        address.Equals(money).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that a value object is equal to itself.
    /// </summary>
    [Fact]
    public void Equals_WithSameInstance_Should_ReturnTrue()
    {
        // Arrange
        var address = new Address("123 Main St", "New York", "10001");

        // Act & Assert
        address.Equals(address).Should().BeTrue();
    }

    #endregion

    #region Operator Tests

    /// <summary>
    /// Verifies that the equality operator returns true for value objects with the same values.
    /// </summary>
    [Fact]
    public void EqualityOperator_WithSameValues_Should_ReturnTrue()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(100.50m, "USD");

        // Act & Assert
        (money1 == money2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that the equality operator returns false for value objects with different values.
    /// </summary>
    [Fact]
    public void EqualityOperator_WithDifferentValues_Should_ReturnFalse()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(200.50m, "USD");

        // Act & Assert
        (money1 == money2).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that the equality operator handles null comparisons correctly.
    /// </summary>
    [Fact]
    public void EqualityOperator_WithNull_Should_ReturnFalse()
    {
        // Arrange
        var money = new Money(100.50m, "USD");

        // Act & Assert
        (money == null).Should().BeFalse();
        (null == money).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that the equality operator returns true when both operands are null.
    /// </summary>
    [Fact]
    public void EqualityOperator_BothNull_Should_ReturnTrue()
    {
        // Arrange
        Money? money1 = null;
        Money? money2 = null;

        // Act & Assert
        (money1 == money2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that the inequality operator returns true for value objects with different values.
    /// </summary>
    [Fact]
    public void InequalityOperator_WithDifferentValues_Should_ReturnTrue()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(100.50m, "EUR");

        // Act & Assert
        (money1 != money2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that the inequality operator returns false for value objects with same values.
    /// </summary>
    [Fact]
    public void InequalityOperator_WithSameValues_Should_ReturnFalse()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(100.50m, "USD");

        // Act & Assert
        (money1 != money2).Should().BeFalse();
    }

    #endregion

    #region GetHashCode Tests

    /// <summary>
    /// Verifies that value objects with the same values produce the same hash code.
    /// </summary>
    [Fact]
    public void GetHashCode_WithSameValues_Should_ReturnSameHashCode()
    {
        // Arrange
        var address1 = new Address("123 Main St", "New York", "10001");
        var address2 = new Address("123 Main St", "New York", "10001");

        // Act & Assert
        address1.GetHashCode().Should().Be(address2.GetHashCode());
    }

    /// <summary>
    /// Verifies that value objects with different values produce different hash codes (with high probability).
    /// </summary>
    [Fact]
    public void GetHashCode_WithDifferentValues_Should_ReturnDifferentHashCodes()
    {
        // Arrange
        var address1 = new Address("123 Main St", "New York", "10001");
        var address2 = new Address("456 Oak Ave", "Los Angeles", "90001");

        // Act & Assert
        address1.GetHashCode().Should().NotBe(address2.GetHashCode());
    }

    /// <summary>
    /// Verifies that hash code is consistent across multiple calls.
    /// </summary>
    [Fact]
    public void GetHashCode_Should_BeConsistent()
    {
        // Arrange
        var money = new Money(99.99m, "GBP");

        // Act
        var hashCode1 = money.GetHashCode();
        var hashCode2 = money.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    /// <summary>
    /// Verifies that value objects with a single atomic value work correctly.
    /// </summary>
    [Fact]
    public void GetHashCode_SingleValue_Should_Work()
    {
        // Arrange
        var value1 = new SingleValue("test");
        var value2 = new SingleValue("test");

        // Act & Assert
        value1.GetHashCode().Should().Be(value2.GetHashCode());
        value1.Equals(value2).Should().BeTrue();
    }

    #endregion

    #region Complex Scenarios

    /// <summary>
    /// Verifies that value objects can be used as dictionary keys.
    /// </summary>
    [Fact]
    public void ValueObject_Should_WorkAsDictionaryKey()
    {
        // Arrange
        var address1 = new Address("123 Main St", "New York", "10001");
        var address2 = new Address("123 Main St", "New York", "10001");
        var dictionary = new Dictionary<Address, string>();

        // Act
        dictionary[address1] = "Home";

        // Assert
        dictionary[address2].Should().Be("Home");
    }

    /// <summary>
    /// Verifies that value objects work correctly in HashSets.
    /// </summary>
    [Fact]
    public void ValueObject_Should_WorkInHashSet()
    {
        // Arrange
        var address1 = new Address("123 Main St", "New York", "10001");
        var address2 = new Address("123 Main St", "New York", "10001");
        var hashSet = new HashSet<Address>();

        // Act
        hashSet.Add(address1);
        hashSet.Add(address2);

        // Assert
        hashSet.Should().ContainSingle();
    }

    #endregion
}
