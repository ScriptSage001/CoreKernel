using CoreKernel.Functional.Extensions;
using CoreKernel.Functional.Maybe;
using FluentAssertions;

namespace CoreKernel.Functional.Tests;

/// <summary>
/// Contains unit tests for the <see cref="MaybeLinqExtensions"/> methods.
/// </summary>
public class MaybeLinqExtensionsTests
{
    #region Select Tests

    /// <summary>
    /// Tests that the Select method transforms the value when the Maybe has a value.
    /// </summary>
    [Fact]
    public void Select_WithValue_Should_TransformValue()
    {
        var maybe = Maybe<int>.Some(5);
        var result = maybe.Select(x => x * 2);
        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(10);
    }

    /// <summary>
    /// Tests that the Select method returns None when the Maybe has no value.
    /// </summary>
    [Fact]
    public void Select_WithNoValue_Should_ReturnNone()
    {
        var maybe = Maybe<int>.None;
        var result = maybe.Select(x => x * 2);
        result.HasValue.Should().BeFalse();
    }

    #endregion

    #region SelectMany Tests

    /// <summary>
    /// Tests that the SelectMany method transforms and flattens the value when the Maybe has a value.
    /// </summary>
    [Fact]
    public void SelectMany_WithValue_Should_TransformAndFlatten()
    {
        var maybe = Maybe<int>.Some(5);
        var result = maybe.SelectMany(x => Maybe<string>.Some($"Value: {x}"));
        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be("Value: 5");
    }

    /// <summary>
    /// Tests that the SelectMany method returns None when the Maybe has no value.
    /// </summary>
    [Fact]
    public void SelectMany_WithNoValue_Should_ReturnNone()
    {
        var maybe = Maybe<int>.None;
        var result = maybe.SelectMany(x => Maybe<string>.Some($"Value: {x}"));
        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the SelectMany method transforms the value using a collection selector and result selector.
    /// </summary>
    [Fact]
    public void SelectMany_WithValue_ReturnsTransformedValue()
    {
        // Arrange
        var source = Maybe<int>.Some(5);
        Func<int, Maybe<string>> collectionSelector = x => Maybe<string>.Some($"Value: {x}");
        Func<int, string, string> resultSelector = (x, y) => $"{y} (Original: {x})";

        // Act
        var result = source.SelectMany(collectionSelector, resultSelector);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal("Value: 5 (Original: 5)", result.ValueOrThrow());
    }

    /// <summary>
    /// Tests that the SelectMany method returns None when the source Maybe has no value.
    /// </summary>
    [Fact]
    public void SelectMany_WithNoValue_ReturnsNone()
    {
        // Arrange
        var source = Maybe<int>.None;
        Func<int, Maybe<string>> collectionSelector = x => Maybe<string>.Some($"Value: {x}");
        Func<int, string, string> resultSelector = (x, y) => $"{y} (Original: {x})";

        // Act
        var result = source.SelectMany(collectionSelector, resultSelector);

        // Assert
        Assert.False(result.HasValue);
    }

    /// <summary>
    /// Tests that the SelectMany method returns None when the intermediate Maybe is None.
    /// </summary>
    [Fact]
    public void SelectMany_WithIntermediateNone_ReturnsNone()
    {
        // Arrange
        var source = Maybe<int>.Some(5);
        Func<int, Maybe<string>> collectionSelector = x => Maybe<string>.None;
        Func<int, string, string> resultSelector = (x, y) => $"{y} (Original: {x})";

        // Act
        var result = source.SelectMany(collectionSelector, resultSelector);

        // Assert
        Assert.False(result.HasValue);
    }

    #endregion

    #region Where Tests

    /// <summary>
    /// Tests that the Where method returns Some when the value satisfies the predicate.
    /// </summary>
    [Fact]
    public void Where_WithValueSatisfyingPredicate_Should_ReturnSome()
    {
        var maybe = Maybe<int>.Some(5);
        var result = maybe.Where(x => x > 3);
        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(5);
    }

    /// <summary>
    /// Tests that the Where method returns None when the value does not satisfy the predicate.
    /// </summary>
    [Fact]
    public void Where_WithValueNotSatisfyingPredicate_Should_ReturnNone()
    {
        var maybe = Maybe<int>.Some(2);
        var result = maybe.Where(x => x > 3);
        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the Where method returns None when the Maybe has no value.
    /// </summary>
    [Fact]
    public void Where_WithNoValue_Should_ReturnNone()
    {
        var maybe = Maybe<int>.None;
        var result = maybe.Where(x => x > 3);
        result.HasValue.Should().BeFalse();
    }

    #endregion

    #region Any Tests

    /// <summary>
    /// Tests that the Any method returns true when the value satisfies the predicate.
    /// </summary>
    [Fact]
    public void Any_WithValueSatisfyingPredicate_Should_ReturnTrue()
    {
        var maybe = Maybe<int>.Some(5);
        var result = maybe.Any(x => x > 3);
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the Any method returns false when the value does not satisfy the predicate.
    /// </summary>
    [Fact]
    public void Any_WithValueNotSatisfyingPredicate_Should_ReturnFalse()
    {
        var maybe = Maybe<int>.Some(2);
        var result = maybe.Any(x => x > 3);
        result.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the Any method returns false when the Maybe has no value.
    /// </summary>
    [Fact]
    public void Any_WithNoValue_Should_ReturnFalse()
    {
        var maybe = Maybe<int>.None;
        var result = maybe.Any(x => x > 3);
        result.Should().BeFalse();
    }

    #endregion

    #region FirstOrDefault Tests

    /// <summary>
    /// Tests that the FirstOrDefault method returns the value when the Maybe has a value.
    /// </summary>
    [Fact]
    public void FirstOrDefault_WithValue_Should_ReturnValue()
    {
        var maybe = Maybe<int>.Some(5);
        var result = maybe.FirstOrDefault(0);
        result.Should().Be(5);
    }

    /// <summary>
    /// Tests that the FirstOrDefault method returns the default value when the Maybe has no value.
    /// </summary>
    [Fact]
    public void FirstOrDefault_WithNoValue_Should_ReturnDefaultValue()
    {
        var maybe = Maybe<int>.None;
        var result = maybe.FirstOrDefault(0);
        result.Should().Be(0);
    }

    #endregion

    #region SingleOrDefault Tests

    /// <summary>
    /// Tests that the SingleOrDefault method returns the value when the Maybe has a value.
    /// </summary>
    [Fact]
    public void SingleOrDefault_WithValue_Should_ReturnValue()
    {
        var maybe = Maybe<int>.Some(5);
        var result = maybe.SingleOrDefault(0);
        result.Should().Be(5);
    }

    /// <summary>
    /// Tests that the SingleOrDefault method returns the default value when the Maybe has no value.
    /// </summary>
    [Fact]
    public void SingleOrDefault_WithNoValue_Should_ReturnDefaultValue()
    {
        var maybe = Maybe<int>.None;
        var result = maybe.SingleOrDefault(0);
        result.Should().Be(0);
    }

    #endregion
}