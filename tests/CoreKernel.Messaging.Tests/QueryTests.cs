using CoreKernel.Functional.Results;
using CoreKernel.Messaging.Queries;
using FluentAssertions;
using MediatR;

namespace CoreKernel.Messaging.Tests;

/// <summary>
/// Contains unit tests for the query interface <see cref="IQuery{TResponse}"/>.
/// These tests verify that the interface correctly extends MediatR's IRequest interface.
/// </summary>
public class QueryTests
{
    #region Test Queries

    /// <summary>
    /// A test query that returns a single entity.
    /// </summary>
    private class GetUserByIdQuery : IQuery<UserDto>
    {
        public Guid UserId { get; }

        public GetUserByIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// A test query that returns a collection.
    /// </summary>
    private class GetAllOrdersQuery : IQuery<IEnumerable<OrderDto>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }

        public GetAllOrdersQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    /// <summary>
    /// A test query that returns a primitive type.
    /// </summary>
    private class GetOrderCountQuery : IQuery<int>
    {
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }

        public GetOrderCountQuery(DateTime? fromDate = null, DateTime? toDate = null)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }
    }

    /// <summary>
    /// A test query that returns a boolean.
    /// </summary>
    private class CheckUserExistsQuery : IQuery<bool>
    {
        public string Email { get; }

        public CheckUserExistsQuery(string email)
        {
            Email = email;
        }
    }

    /// <summary>
    /// A sample DTO for testing.
    /// </summary>
    private class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// A sample DTO for testing.
    /// </summary>
    private class OrderDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    #endregion

    #region IQuery<TResponse> Tests

    /// <summary>
    /// Verifies that <see cref="IQuery{TResponse}"/> implements <see cref="IRequest{TResponse}"/> with <see cref="Result{TResponse}"/> as response.
    /// </summary>
    [Fact]
    public void IQuery_Should_ImplementIRequestWithResultT()
    {
        // Assert
        typeof(IQuery<UserDto>).Should().BeAssignableTo<IRequest<Result<UserDto>>>();
    }

    /// <summary>
    /// Verifies that a custom query implementing <see cref="IQuery{TResponse}"/> is assignable to the interface.
    /// </summary>
    [Fact]
    public void CustomQuery_Should_ImplementIQuery()
    {
        // Arrange
        var query = new GetUserByIdQuery(Guid.NewGuid());

        // Assert
        query.Should().BeAssignableTo<IQuery<UserDto>>();
        query.Should().BeAssignableTo<IRequest<Result<UserDto>>>();
    }

    /// <summary>
    /// Verifies that a query can store and expose its properties correctly.
    /// </summary>
    [Fact]
    public void Query_Should_StorePropertiesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var query = new GetUserByIdQuery(userId);

        // Assert
        query.UserId.Should().Be(userId);
    }

    #endregion

    #region Collection Query Tests

    /// <summary>
    /// Verifies that queries returning collections are correctly typed.
    /// </summary>
    [Fact]
    public void QueryWithCollectionResponse_Should_ImplementCorrectInterface()
    {
        // Arrange
        var query = new GetAllOrdersQuery(1, 10);

        // Assert
        query.Should().BeAssignableTo<IQuery<IEnumerable<OrderDto>>>();
        query.Should().BeAssignableTo<IRequest<Result<IEnumerable<OrderDto>>>>();
    }

    /// <summary>
    /// Verifies that a query with pagination parameters stores values correctly.
    /// </summary>
    [Fact]
    public void QueryWithPagination_Should_StoreParametersCorrectly()
    {
        // Arrange
        var pageNumber = 5;
        var pageSize = 20;

        // Act
        var query = new GetAllOrdersQuery(pageNumber, pageSize);

        // Assert
        query.PageNumber.Should().Be(pageNumber);
        query.PageSize.Should().Be(pageSize);
    }

    #endregion

    #region Primitive Type Response Tests

    /// <summary>
    /// Verifies that queries returning primitive types are correctly typed.
    /// </summary>
    [Fact]
    public void QueryWithPrimitiveResponse_Should_ImplementCorrectInterface()
    {
        // Arrange
        var query = new GetOrderCountQuery();

        // Assert
        query.Should().BeAssignableTo<IQuery<int>>();
        query.Should().BeAssignableTo<IRequest<Result<int>>>();
    }

    /// <summary>
    /// Verifies that queries returning boolean are correctly typed.
    /// </summary>
    [Fact]
    public void QueryWithBooleanResponse_Should_ImplementCorrectInterface()
    {
        // Arrange
        var query = new CheckUserExistsQuery("test@example.com");

        // Assert
        query.Should().BeAssignableTo<IQuery<bool>>();
        query.Should().BeAssignableTo<IRequest<Result<bool>>>();
    }

    /// <summary>
    /// Verifies that a query with optional parameters handles nulls correctly.
    /// </summary>
    [Fact]
    public void QueryWithOptionalParameters_Should_HandleNulls()
    {
        // Arrange & Act
        var query = new GetOrderCountQuery();

        // Assert
        query.FromDate.Should().BeNull();
        query.ToDate.Should().BeNull();
    }

    /// <summary>
    /// Verifies that a query with optional parameters stores provided values correctly.
    /// </summary>
    [Fact]
    public void QueryWithOptionalParameters_Should_StoreProvidedValues()
    {
        // Arrange
        var fromDate = new DateTime(2024, 1, 1);
        var toDate = new DateTime(2024, 12, 31);

        // Act
        var query = new GetOrderCountQuery(fromDate, toDate);

        // Assert
        query.FromDate.Should().Be(fromDate);
        query.ToDate.Should().Be(toDate);
    }

    #endregion

    #region Interface Structure Tests

    /// <summary>
    /// Verifies that <see cref="IQuery{TResponse}"/> is a marker interface without additional members.
    /// </summary>
    [Fact]
    public void IQuery_Should_BeMarkerInterface()
    {
        // Assert - IQuery<T> only extends IRequest<Result<T>>, no additional members
        typeof(IQuery<string>).GetInterfaces().Should().Contain(typeof(IRequest<Result<string>>));
    }

    /// <summary>
    /// Verifies that IQuery works correctly with various generic type arguments.
    /// </summary>
    [Fact]
    public void IQuery_Should_SupportVariousGenericTypes()
    {
        // Assert
        typeof(IQuery<int>).Should().BeAssignableTo<IRequest<Result<int>>>();
        typeof(IQuery<string>).Should().BeAssignableTo<IRequest<Result<string>>>();
        typeof(IQuery<Guid>).Should().BeAssignableTo<IRequest<Result<Guid>>>();
        typeof(IQuery<List<int>>).Should().BeAssignableTo<IRequest<Result<List<int>>>>();
    }

    #endregion
}
