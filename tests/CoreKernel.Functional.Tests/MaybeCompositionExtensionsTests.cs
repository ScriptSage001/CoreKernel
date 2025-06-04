using CoreKernel.Functional.Extensions;
using CoreKernel.Functional.Maybe;
using FluentAssertions;

namespace CoreKernel.Functional.Tests;

/// <summary>
/// Contains unit tests for the <see cref="MaybeCompositionExtensions"/> methods.
/// </summary>
public class MaybeCompositionExtensionsTests
{
    #region Or Tests

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Or{T}"/> method returns the original Maybe
    /// when it has a value, even if an alternate Maybe is provided.
    /// </summary>
    [Fact]
    public void Or_WithValue_Should_ReturnOriginalMaybe()
    {
        var maybe = Maybe<int>.Some(5);
        var alternate = Maybe<int>.Some(10);

        var result = maybe.Or(alternate);

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(5);
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Or{T}"/> method returns the alternate Maybe
    /// when the original Maybe has no value.
    /// </summary>
    [Fact]
    public void Or_WithNoValue_Should_ReturnAlternateMaybe()
    {
        var maybe = Maybe<int>.None;
        var alternate = Maybe<int>.Some(10);

        var result = maybe.Or(alternate);

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(10);
    }

    #endregion

    #region Or (Alternate Value) Tests

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Or{T}"/> method returns the original Maybe
    /// when it has a value, even if an alternate value is provided.
    /// </summary>
    [Fact]
    public void Or_WithValue_Should_ReturnOriginalMaybe_WhenAlternateValueProvided()
    {
        var maybe = Maybe<int>.Some(5);

        var result = maybe.Or(10);

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(5);
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Or{T}"/> method returns a Maybe containing
    /// the alternate value when the original Maybe has no value.
    /// </summary>
    [Fact]
    public void Or_WithNoValue_Should_ReturnMaybeWithAlternateValue()
    {
        var maybe = Maybe<int>.None;

        var result = maybe.Or(10);

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(10);
    }

    #endregion

    #region OrElse Tests

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.OrElse{T}"/> method returns the original Maybe
    /// when it has a value, even if a factory for an alternate Maybe is provided.
    /// </summary>
    [Fact]
    public void OrElse_WithValue_Should_ReturnOriginalMaybe()
    {
        var maybe = Maybe<int>.Some(5);

        var result = maybe.OrElse(() => Maybe<int>.Some(10));

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(5);
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.OrElse{T}"/> method returns the alternate Maybe
    /// produced by the factory when the original Maybe has no value.
    /// </summary>
    [Fact]
    public void OrElse_WithNoValue_Should_ReturnAlternateMaybeFromFactory()
    {
        var maybe = Maybe<int>.None;

        var result = maybe.OrElse(() => Maybe<int>.Some(10));

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(10);
    }

    #endregion

    #region Flatten Tests

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Flatten{T}"/> method returns the inner Maybe
    /// when the outer Maybe contains a value.
    /// </summary>
    [Fact]
    public void Flatten_WithNestedSome_Should_ReturnInnerMaybe()
    {
        var nestedMaybe = Maybe<Maybe<int>>.Some(Maybe<int>.Some(5));

        var result = nestedMaybe.Flatten();

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(5);
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Flatten{T}"/> method returns None
    /// when the outer Maybe contains a None value.
    /// </summary>
    [Fact]
    public void Flatten_WithNestedNone_Should_ReturnNone()
    {
        var nestedMaybe = Maybe<Maybe<int>>.Some(Maybe<int>.None);

        var result = nestedMaybe.Flatten();

        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Flatten{T}"/> method returns None
    /// when the outer Maybe itself has no value.
    /// </summary>
    [Fact]
    public void Flatten_WithNoValue_Should_ReturnNone()
    {
        var nestedMaybe = Maybe<Maybe<int>>.None;

        var result = nestedMaybe.Flatten();

        result.HasValue.Should().BeFalse();
    }

    #endregion

    #region Zip Tests

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Zip{T1, T2}"/> method returns a Maybe containing
    /// a tuple of values when both Maybes have values.
    /// </summary>
    [Fact]
    public void Zip_WithBothValues_Should_ReturnTuple()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<string>.Some("Hello");

        var result = first.Zip(second);

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be((5, "Hello"));
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Zip{T1, T2}"/> method returns None
    /// when the first Maybe has no value.
    /// </summary>
    [Fact]
    public void Zip_WithFirstNone_Should_ReturnNone()
    {
        var first = Maybe<int>.None;
        var second = Maybe<string>.Some("Hello");

        var result = first.Zip(second);

        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Zip{T1, T2}"/> method returns None
    /// when the second Maybe has no value.
    /// </summary>
    [Fact]
    public void Zip_WithSecondNone_Should_ReturnNone()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<string>.None;

        var result = first.Zip(second);

        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Zip{T1, T2, T3}"/> method returns a Maybe containing
    /// a tuple of values when all three Maybes have values.
    /// </summary>
    [Fact]
    public void Zip_WithThreeValues_Should_ReturnTuple()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<string>.Some("Hello");
        var third = Maybe<double>.Some(3.14);

        var result = first.Zip(second, third);

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be((5, "Hello", 3.14));
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Zip{T1, T2, T3}"/> method returns None
    /// when the first Maybe has no value.
    /// </summary>
    [Fact]
    public void Zip_ForThreeMaybe_WithFirstNone_Should_ReturnNone()
    {
        var first = Maybe<int>.None;
        var second = Maybe<string>.Some("Hello");
        var third = Maybe<double>.Some(3.14);

        var result = first.Zip(second, third);

        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Zip{T1, T2, T3}"/> method returns None
    /// when the second Maybe has no value.
    /// </summary>
    [Fact]
    public void Zip_ForThreeMaybe_WithSecondNone_Should_ReturnNone()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<string>.None;
        var third = Maybe<double>.Some(3.14);

        var result = first.Zip(second, third);

        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Zip{T1, T2, T3}"/> method returns None
    /// when the third Maybe has no value.
    /// </summary>
    [Fact]
    public void Zip_ForThreeMaybe_WithThirdNone_Should_ReturnNone()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<string>.Some("Hello");
        var third = Maybe<double>.None;

        var result = first.Zip(second, third);

        result.HasValue.Should().BeFalse();
    }
    
    #endregion

    #region Apply Tests

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Apply{T1, T2, TResult}"/> method applies the provided
    /// function to the values of both Maybes when both have values.
    /// </summary>
    [Fact]
    public void Apply_WithBothValues_Should_ApplyFunction()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<int>.Some(10);

        var result = first.Apply(second, (x, y) => x + y);

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(15);
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Apply{T1, T2, TResult}"/> method returns None
    /// when the first Maybe has no value.
    /// </summary>
    [Fact]
    public void Apply_WithFirstNone_Should_ReturnNone()
    {
        var first = Maybe<int>.None;
        var second = Maybe<int>.Some(10);

        var result = first.Apply(second, (x, y) => x + y);

        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Apply{T1, T2, TResult}"/> method returns None
    /// when the second Maybe has no value.
    /// </summary>
    [Fact]
    public void Apply_WithSecondNone_Should_ReturnNone()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<int>.None;

        var result = first.Apply(second, (x, y) => x + y);

        result.HasValue.Should().BeFalse();
    }

     /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Apply{T1, T2, T3, TResult}"/> method applies the provided
    /// function to the values of all three Maybes when all have values.
    /// </summary>
    [Fact]
    public void Apply_WithThreeValues_Should_ApplyFunction()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<int>.Some(10);
        var third = Maybe<int>.Some(15);

        var result = first.Apply(second, third, (x, y, z) => x + y + z);

        result.HasValue.Should().BeTrue();
        result.ValueOrThrow().Should().Be(30);
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Apply{T1, T2, T3, TResult}"/> method returns None
    /// when the first Maybe has no value.
    /// </summary>
    [Fact]
    public void Apply_ForThreeMaybe_WithFirstNone_Should_ReturnNone()
    {
        var first = Maybe<int>.None;
        var second = Maybe<int>.Some(10);
        var third = Maybe<int>.Some(15);

        var result = first.Apply(second, third, (x, y, z) => x + y + z);

        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Apply{T1, T2, T3, TResult}"/> method returns None
    /// when the second Maybe has no value.
    /// </summary>
    [Fact]
    public void Apply_ForThreeMaybe_WithSecondNone_Should_ReturnNone()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<int>.None;
        var third = Maybe<int>.Some(15);

        var result = first.Apply(second, third, (x, y, z) => x + y + z);

        result.HasValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the <see cref="MaybeCompositionExtensions.Apply{T1, T2, T3, TResult}"/> method returns None
    /// when the third Maybe has no value.
    /// </summary>
    [Fact]
    public void Apply_ForThreeMaybe_WithThirdNone_Should_ReturnNone()
    {
        var first = Maybe<int>.Some(5);
        var second = Maybe<int>.Some(10);
        var third = Maybe<int>.None;

        var result = first.Apply(second, third, (x, y, z) => x + y + z);

        result.HasValue.Should().BeFalse();
    }
    
    #endregion
}