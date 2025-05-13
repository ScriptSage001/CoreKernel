using CoreKernel.Functional.Extensions;
using CoreKernel.Functional.Maybe;
using CoreKernel.Functional.Results;
using FluentAssertions;

namespace CoreKernel.Functional.Tests;

/// <summary>
/// Contains unit tests for the <see cref="Maybe{T}"/> struct.
/// </summary>
public class MaybeTests
{
    #region Some Tests

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Some(T)"/> creates a <see cref="Maybe{T}"/> instance with a value.
    /// </summary>
    [Fact]
    public void Some_Should_CreateMaybeWithValue()
    {
        var maybe = Maybe<string>.Some("test");
        maybe.HasValue.Should().BeTrue();
        maybe.ValueOrThrow().Should().Be("test");
    }

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Some(T)"/> throws an <see cref="ArgumentNullException"/> when a null value is provided.
    /// </summary>
    [Fact]
    public void Some_WithNullValue_Should_ThrowArgumentNullException()
    {
        string? nullValue = null;
        var exception = Assert.Throws<ArgumentNullException>(() => Maybe<string>.Some(nullValue!));
        exception.ParamName.Should().Be("value");
    }

    #endregion

    #region None Tests

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.None"/> creates an empty <see cref="Maybe{T}"/> instance.
    /// </summary>
    [Fact]
    public void None_Should_CreateEmptyMaybe()
    {
        var maybe = Maybe<string>.None;
        maybe.HasValue.Should().BeFalse();
    }

    #endregion

    #region ValueOrThrow Tests

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.ValueOrThrow"/> returns the value when it is present.
    /// </summary>
    [Fact]
    public void ValueOrThrow_WithValue_Should_ReturnValue()
    {
        var maybe = Maybe<string>.Some("test");
        var value = maybe.ValueOrThrow();
        value.Should().Be("test");
    }

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.ValueOrThrow"/> throws an <see cref="InvalidOperationException"/> when no value is present.
    /// </summary>
    [Fact]
    public void ValueOrThrow_WithNoValue_Should_ThrowInvalidOperationException()
    {
        var maybe = Maybe<string>.None;
        var exception = Assert.Throws<InvalidOperationException>(() => maybe.ValueOrThrow());
        exception.Message.Should().Contain("No value present");
    }

    #endregion

    #region ValueOrDefault Tests

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.ValueOrDefault"/> returns the value when it is present.
    /// </summary>
    [Fact]
    public void ValueOrDefault_WithValue_Should_ReturnValue()
    {
        var maybe = Maybe<string>.Some("test");
        var value = maybe.ValueOrDefault();
        value.Should().Be("test");
    }

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.ValueOrDefault"/> returns the default value when no value is present.
    /// </summary>
    [Fact]
    public void ValueOrDefault_WithNoValue_Should_ReturnDefault()
    {
        var maybe = Maybe<string>.None;
        var value = maybe.ValueOrDefault();
        value.Should().BeNull();
    }

    #endregion

    #region Match Tests

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Match(Action{T}, Action)"/> executes the "onSome" action when a value is present.
    /// </summary>
    [Fact]
    public void Match_WithValue_Should_ExecuteOnSome()
    {
        var maybe = Maybe<int>.Some(5);
        var onSomeExecuted = false;
        var onNoneExecuted = false;

        maybe.Match(
            value => onSomeExecuted = true,
            () => onNoneExecuted = true
        );

        onSomeExecuted.Should().BeTrue();
        onNoneExecuted.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Match(Action{T}, Action)"/> executes the "onNone" action when no value is present.
    /// </summary>
    [Fact]
    public void Match_WithNoValue_Should_ExecuteOnNone()
    {
        var maybe = Maybe<int>.None;
        var onSomeExecuted = false;
        var onNoneExecuted = false;

        maybe.Match(
            value => onSomeExecuted = true,
            () => onNoneExecuted = true
        );

        onSomeExecuted.Should().BeFalse();
        onNoneExecuted.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Match{TResult}(Func{T, TResult}, Func{TResult})"/> returns the result of the "onSome" function when a value is present.
    /// </summary>
    [Fact]
    public void Match_WithValue_Should_ReturnOnSomeResult()
    {
        var maybe = Maybe<int>.Some(5);
        var result = maybe.Match(
            value => $"Value is {value}",
            () => "No value"
        );
        result.Should().Be("Value is 5");
    }

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Match{TResult}(Func{T, TResult}, Func{TResult})"/> returns the result of the "onNone" function when no value is present.
    /// </summary>
    [Fact]
    public void Match_WithNoValue_Should_ReturnOnNoneResult()
    {
        var maybe = Maybe<int>.None;
        var result = maybe.Match(
            value => $"Value is {value}",
            () => "No value"
        );
        result.Should().Be("No value");
    }

    #endregion

    #region Map Tests

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Map{TResult}(Func{T, TResult})"/> transforms the value when it is present.
    /// </summary>
    [Fact]
    public void Map_WithValue_Should_TransformValue()
    {
        var maybe = Maybe<int>.Some(5);
        var mapped = maybe.Map(value => value * 2);
        mapped.HasValue.Should().BeTrue();
        mapped.ValueOrThrow().Should().Be(10);
    }

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Map{TResult}(Func{T, TResult})"/> returns <see cref="Maybe{T}.None"/> when no value is present.
    /// </summary>
    [Fact]
    public void Map_WithNoValue_Should_ReturnNone()
    {
        var maybe = Maybe<int>.None;
        var mapped = maybe.Map(value => value * 2);
        mapped.HasValue.Should().BeFalse();
    }

    #endregion

    #region Bind Tests

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Bind{TResult}(Func{T, Maybe{TResult}})"/> returns the result of the binder function when a value is present.
    /// </summary>
    [Fact]
    public void Bind_WithValue_Should_ReturnBinderResult()
    {
        var maybe = Maybe<int>.Some(5);
        var bound = maybe.Bind(value => value > 0
            ? Maybe<string>.Some($"Positive: {value}")
            : Maybe<string>.None);
        bound.HasValue.Should().BeTrue();
        bound.ValueOrThrow().Should().Be("Positive: 5");
    }

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.Bind{TResult}(Func{T, Maybe{TResult}})"/> returns <see cref="Maybe{T}.None"/> when no value is present.
    /// </summary>
    [Fact]
    public void Bind_WithNoValue_Should_ReturnNone()
    {
        var maybe = Maybe<int>.None;
        var bound = maybe.Bind(value => value > 0
            ? Maybe<string>.Some($"Positive: {value}")
            : Maybe<string>.None);
        bound.HasValue.Should().BeFalse();
    }

    #endregion

    #region Implicit Conversion Tests

    /// <summary>
    /// Verifies that implicit conversion from a value creates a <see cref="Maybe{T}.Some(T)"/>.
    /// </summary>
    [Fact]
    public void ImplicitConversion_FromValue_Should_CreateSome()
    {
        var value = "test";
        Maybe<string> maybe = value;
        maybe.HasValue.Should().BeTrue();
        maybe.ValueOrThrow().Should().Be("test");
    }

    /// <summary>
    /// Verifies that implicit conversion from a null value creates <see cref="Maybe{T}.None"/>.
    /// </summary>
    [Fact]
    public void ImplicitConversion_FromNull_Should_CreateNone()
    {
        string? value = null;
        Maybe<string> maybe = value;
        maybe.HasValue.Should().BeFalse();
    }

    #endregion

    #region Equals Tests

    /// <summary>
    /// Verifies that two <see cref="Maybe{T}"/> instances with the same values are equal.
    /// </summary>
    [Fact]
    public void Equals_WithSameValues_Should_BeEqual()
    {
        var maybe1 = Maybe<string>.Some("test");
        var maybe2 = Maybe<string>.Some("test");
        maybe1.Equals(maybe2).Should().BeTrue();
        (maybe1 == maybe2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that two <see cref="Maybe{T}"/> instances with different values are not equal.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentValues_Should_NotBeEqual()
    {
        var maybe1 = Maybe<string>.Some("test1");
        var maybe2 = Maybe<string>.Some("test2");
        maybe1.Equals(maybe2).Should().BeFalse();
        (maybe1 != maybe2).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that two <see cref="Maybe{T}.None"/> instances are equal.
    /// </summary>
    [Fact]
    public void Equals_BothNone_Should_BeEqual()
    {
        var maybe1 = Maybe<string>.None;
        var maybe2 = Maybe<string>.None;
        maybe1.Equals(maybe2).Should().BeTrue();
        (maybe1 == maybe2).Should().BeTrue();
    }

    #endregion

    #region ToResult Tests

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.ToResult"/> returns a success result when a value is present.
    /// </summary>
    [Fact]
    public void ToResult_WithValue_Should_ReturnSuccessResult()
    {
        var maybe = Maybe<string>.Some("test");
        var result = maybe.ToResult("Value is required");
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("test");
    }

    /// <summary>
    /// Verifies that <see cref="Maybe{T}.ToResult"/> returns a failure result when no value is present.
    /// </summary>
    [Fact]
    public void ToResult_WithNoValue_Should_ReturnFailureResult()
    {
        var maybe = Maybe<string>.None;
        var errorMessage = "Value is required";
        var result = maybe.ToResult(errorMessage);
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Be(errorMessage);
    }

    #endregion

    #region ToMaybe Tests

    /// <summary>
    /// Verifies that <see cref="Result{T}.ToMaybe"/> returns <see cref="Maybe{T}.Some(T)"/> when the result is successful.
    /// </summary>
    [Fact]
    public void ToMaybe_WithSuccessResult_Should_ReturnSome()
    {
        var result = Result.Success("test");
        var maybe = result.ToMaybe();
        maybe.HasValue.Should().BeTrue();
        maybe.ValueOrThrow().Should().Be("test");
    }

    /// <summary>
    /// Verifies that <see cref="Result{T}.ToMaybe"/> returns <see cref="Maybe{T}.None"/> when the result is a failure.
    /// </summary>
    [Fact]
    public void ToMaybe_WithFailureResult_Should_ReturnNone()
    {
        var result = Result.Failure<string>(Error.ValidationError);
        var maybe = result.ToMaybe();
        maybe.HasValue.Should().BeFalse();
    }

    #endregion
}