using CoreKernel.Functional.Extensions;
using CoreKernel.Functional.Maybe;
using FluentAssertions;

namespace CoreKernel.Functional.Tests;

/// <summary>
/// Contains unit tests for the <see cref="MaybeBooleanExtensions"/> methods.
/// </summary>
public class MaybeBooleanExtensionsTests
{
    #region Then Tests

    [Fact]
    public void Then_WithTrueCondition_Should_ExecuteAction()
    {
        // Arrange
        var executed = false;

        // Act
        var result = true.Then(() => executed = true);

        // Assert
        result.Should().BeTrue();
        executed.Should().BeTrue();
    }

    [Fact]
    public void Then_WithFalseCondition_Should_NotExecuteAction()
    {
        // Arrange
        var executed = false;

        // Act
        var result = false.Then(() => executed = true);

        // Assert
        result.Should().BeFalse();
        executed.Should().BeFalse();
    }

    #endregion

    #region Else Tests

    [Fact]
    public void Else_WithFalseCondition_Should_ExecuteAction()
    {
        // Arrange
        var executed = false;

        // Act
        var result = false.Else(() => executed = true);

        // Assert
        result.Should().BeFalse();
        executed.Should().BeTrue();
    }

    [Fact]
    public void Else_WithTrueCondition_Should_NotExecuteAction()
    {
        // Arrange
        var executed = false;

        // Act
        var result = true.Else(() => executed = true);

        // Assert
        result.Should().BeTrue();
        executed.Should().BeFalse();
    }

    #endregion

    #region ToMaybe Tests

    [Fact]
    public void ToMaybe_WithTrueCondition_Should_ReturnSome()
    {
        // Act
        var result = true.ToMaybe(5);

        // Assert
        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(5);
    }

    [Fact]
    public void ToMaybe_WithFalseCondition_Should_ReturnNone()
    {
        // Act
        var result = false.ToMaybe(5);

        // Assert
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public void ToMaybe_WithTrueConditionAndFactory_Should_ReturnSome()
    {
        // Act
        var result = true.ToMaybe(() => 5);

        // Assert
        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(5);
    }

    [Fact]
    public void ToMaybe_WithFalseConditionAndFactory_Should_ReturnNone()
    {
        // Act
        var result = false.ToMaybe(() => 5);

        // Assert
        result.HasValue.Should().BeFalse();
    }

    #endregion

    #region Match Tests

    [Fact]
    public void Match_WithTrueCondition_Should_ExecuteOnTrueAction()
    {
        // Arrange
        var executedTrue = false;
        var executedFalse = false;

        // Act
        var result = true.Match(() => executedTrue = true, () => executedFalse = true);

        // Assert
        result.Should().BeTrue();
        executedTrue.Should().BeTrue();
        executedFalse.Should().BeFalse();
    }

    [Fact]
    public void Match_WithFalseCondition_Should_ExecuteOnFalseAction()
    {
        // Arrange
        var executedTrue = false;
        var executedFalse = false;

        // Act
        var result = false.Match(() => executedTrue = true, () => executedFalse = true);

        // Assert
        result.Should().BeTrue();
        executedTrue.Should().BeFalse();
        executedFalse.Should().BeTrue();
    }

    [Fact]
    public void Match_WithTrueCondition_Should_ReturnOnTrueValue()
    {
        // Act
        var result = true.Match(5, 10);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void Match_WithFalseCondition_Should_ReturnOnFalseValue()
    {
        // Act
        var result = false.Match(5, 10);

        // Assert
        result.Should().Be(10);
    }

    [Fact]
    public void Match_WithTrueConditionAndFactory_Should_ReturnOnTrueValue()
    {
        // Act
        var result = true.Match(() => 5, () => 10);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void Match_WithFalseConditionAndFactory_Should_ReturnOnFalseValue()
    {
        // Act
        var result = false.Match(() => 5, () => 10);

        // Assert
        result.Should().Be(10);
    }

    #endregion

    #region DoWhenTrue Tests

    [Fact]
    public void DoWhenTrue_WithTrueMaybe_Should_ExecuteAction()
    {
        // Arrange
        var executed = false;
        var maybe = Maybe<bool>.Some(true);

        // Act
        var result = maybe.DoWhenTrue(() => executed = true);

        // Assert
        result.HasValue.Should().BeTrue();
        executed.Should().BeTrue();
    }

    [Fact]
    public void DoWhenTrue_WithFalseMaybe_Should_NotExecuteAction()
    {
        // Arrange
        var executed = false;
        var maybe = Maybe<bool>.Some(false);

        // Act
        var result = maybe.DoWhenTrue(() => executed = true);

        // Assert
        result.HasValue.Should().BeTrue();
        executed.Should().BeFalse();
    }

    [Fact]
    public void DoWhenTrue_WithNoneMaybe_Should_NotExecuteAction()
    {
        // Arrange
        var executed = false;
        var maybe = Maybe<bool>.None;

        // Act
        var result = maybe.DoWhenTrue(() => executed = true);

        // Assert
        result.HasValue.Should().BeFalse();
        executed.Should().BeFalse();
    }

    #endregion

    #region DoWhenFalse Tests

    [Fact]
    public void DoWhenFalse_WithFalseMaybe_Should_ExecuteAction()
    {
        // Arrange
        var executed = false;
        var maybe = Maybe<bool>.Some(false);

        // Act
        var result = maybe.DoWhenFalse(() => executed = true);

        // Assert
        result.HasValue.Should().BeTrue();
        executed.Should().BeTrue();
    }

    [Fact]
    public void DoWhenFalse_WithTrueMaybe_Should_NotExecuteAction()
    {
        // Arrange
        var executed = false;
        var maybe = Maybe<bool>.Some(true);

        // Act
        var result = maybe.DoWhenFalse(() => executed = true);

        // Assert
        result.HasValue.Should().BeTrue();
        executed.Should().BeFalse();
    }

    [Fact]
    public void DoWhenFalse_WithNoneMaybe_Should_NotExecuteAction()
    {
        // Arrange
        var executed = false;
        var maybe = Maybe<bool>.None;

        // Act
        var result = maybe.DoWhenFalse(() => executed = true);

        // Assert
        result.HasValue.Should().BeFalse();
        executed.Should().BeFalse();
    }

    #endregion

    #region Filter Tests

    [Fact]
    public void Filter_WithValueSatisfyingPredicate_Should_ReturnSome()
    {
        // Arrange
        var maybe = Maybe<int>.Some(5);

        // Act
        var result = maybe.Filter(x => x > 3);

        // Assert
        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(5);
    }

    [Fact]
    public void Filter_WithValueNotSatisfyingPredicate_Should_ReturnNone()
    {
        // Arrange
        var maybe = Maybe<int>.Some(2);

        // Act
        var result = maybe.Filter(x => x > 3);

        // Assert
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public void Filter_WithNone_Should_ReturnNone()
    {
        // Arrange
        var maybe = Maybe<int>.None;

        // Act
        var result = maybe.Filter(x => x > 3);

        // Assert
        result.HasValue.Should().BeFalse();
    }

    #endregion
}