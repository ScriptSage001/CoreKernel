using CoreKernel.Functional.Maybe;
using FluentAssertions;

namespace CoreKernel.Functional.Tests;

/// <summary>
/// Contains unit tests for the <see cref="MaybeAsync"/> extension methods.
/// </summary>
public class MaybeAsyncTests
{
    #region ToMaybeAsync Tests

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.ToMaybeAsync{T}"/> creates a <see cref="Maybe{T}"/> with a value when the task result is non-null.
    /// </summary>
    [Fact]
    public async Task ToMaybeAsync_WithNonNullValue_Should_ReturnSome()
    {
        var task = Task.FromResult("test");
        var maybe = await task.ToMaybeAsync();
        maybe.HasValue.Should().BeTrue();
        maybe.ValueOrThrow().Should().Be("test");
    }

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.ToMaybeAsync{T}"/> creates a <see cref="Maybe{T}.None"/> when the task result is null.
    /// </summary>
    [Fact]
    public async Task ToMaybeAsync_WithNullValue_Should_ReturnNone()
    {
        var task = Task.FromResult<string?>(null);
        var maybe = await task.ToMaybeAsync();
        maybe.HasValue.Should().BeFalse();
    }

    #endregion

    #region MapAsync Tests

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.MapAsync{T, TResult}"/> transforms the value when it is present.
    /// </summary>
    [Fact]
    public async Task MapAsync_WithValue_Should_TransformValue()
    {
        var maybe = Maybe<int>.Some(5);
        var mapped = await maybe.MapAsync(value => Task.FromResult(value * 2));
        mapped.HasValue.Should().BeTrue();
        mapped.ValueOrThrow().Should().Be(10);
    }

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.MapAsync{T, TResult}"/> returns <see cref="Maybe{T}.None"/> when no value is present.
    /// </summary>
    [Fact]
    public async Task MapAsync_WithNoValue_Should_ReturnNone()
    {
        var maybe = Maybe<int>.None;
        var mapped = await maybe.MapAsync(value => Task.FromResult(value * 2));
        mapped.HasValue.Should().BeFalse();
    }

    #endregion

    #region BindAsync Tests

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.BindAsync{T, TResult}"/> returns the result of the binder function when a value is present.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithValue_Should_ReturnBinderResult()
    {
        var maybe = Maybe<int>.Some(5);
        var bound = await maybe.BindAsync(value => Task.FromResult(
            value > 0 ? Maybe<string>.Some($"Positive: {value}") : Maybe<string>.None));
        bound.HasValue.Should().BeTrue();
        bound.ValueOrThrow().Should().Be("Positive: 5");
    }

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.BindAsync{T, TResult}"/> returns <see cref="Maybe{T}.None"/> when no value is present.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithNoValue_Should_ReturnNone()
    {
        var maybe = Maybe<int>.None;
        var bound = await maybe.BindAsync(value => Task.FromResult(
            value > 0 ? Maybe<string>.Some($"Positive: {value}") : Maybe<string>.None));
        bound.HasValue.Should().BeFalse();
    }

    #endregion

    #region MatchAsync Tests

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.MatchAsync{T, TResult}"/> executes the "onSome" function when a value is present.
    /// </summary>
    [Fact]
    public async Task MatchAsync_WithValue_Should_ExecuteOnSome()
    {
        var maybe = Maybe<int>.Some(5);
        var result = await maybe.MatchAsync(
            value => Task.FromResult($"Value is {value}"),
            () => Task.FromResult("No value"));
        result.Should().Be("Value is 5");
    }

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.MatchAsync{T, TResult}"/> executes the "onNone" function when no value is present.
    /// </summary>
    [Fact]
    public async Task MatchAsync_WithNoValue_Should_ExecuteOnNone()
    {
        var maybe = Maybe<int>.None;
        var result = await maybe.MatchAsync(
            value => Task.FromResult($"Value is {value}"),
            () => Task.FromResult("No value"));
        result.Should().Be("No value");
    }

    #endregion

    #region DoAsync Tests

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.DoAsync{T}"/> executes the action when a value is present.
    /// </summary>
    [Fact]
    public async Task DoAsync_WithValue_Should_ExecuteAction()
    {
        var maybe = Maybe<int>.Some(5);
        var executed = false;

        await maybe.DoAsync(value =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        executed.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that <see cref="MaybeAsync.DoAsync{T}"/> does not execute the action when no value is present.
    /// </summary>
    [Fact]
    public async Task DoAsync_WithNoValue_Should_NotExecuteAction()
    {
        var maybe = Maybe<int>.None;
        var executed = false;

        await maybe.DoAsync(value =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        executed.Should().BeFalse();
    }

    #endregion
}