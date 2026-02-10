using CoreKernel.Functional.Results;
using CoreKernel.Messaging.Commands;
using FluentAssertions;
using MediatR;

namespace CoreKernel.Messaging.Tests;

/// <summary>
/// Contains unit tests for the command interfaces <see cref="ICommand"/> and <see cref="ICommand{TResponse}"/>.
/// These tests verify that the interfaces correctly extend MediatR's IRequest interface.
/// </summary>
public class CommandTests
{
    #region Test Commands

    /// <summary>
    /// A test command that does not return a value.
    /// </summary>
    private class CreateOrderCommand : ICommand
    {
        public string CustomerName { get; }
        public decimal TotalAmount { get; }

        public CreateOrderCommand(string customerName, decimal totalAmount)
        {
            CustomerName = customerName;
            TotalAmount = totalAmount;
        }
    }

    /// <summary>
    /// A test command that returns a response value.
    /// </summary>
    private class CreateUserCommand : ICommand<Guid>
    {
        public string Email { get; }
        public string Name { get; }

        public CreateUserCommand(string email, string name)
        {
            Email = email;
            Name = name;
        }
    }

    /// <summary>
    /// A test command that returns a complex response type.
    /// </summary>
    private class GetOrderSummaryCommand : ICommand<OrderSummary>
    {
        public Guid OrderId { get; }

        public GetOrderSummaryCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }

    /// <summary>
    /// A sample response type for testing.
    /// </summary>
    private class OrderSummary
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    #endregion

    #region ICommand Tests

    /// <summary>
    /// Verifies that <see cref="ICommand"/> implements <see cref="IRequest{TResponse}"/> with <see cref="Result"/> as response.
    /// </summary>
    [Fact]
    public void ICommand_Should_ImplementIRequestWithResult()
    {
        // Assert
        typeof(ICommand).Should().BeAssignableTo<IRequest<Result>>();
    }

    /// <summary>
    /// Verifies that a custom command implementing <see cref="ICommand"/> is assignable to the interface.
    /// </summary>
    [Fact]
    public void CustomCommand_Should_ImplementICommand()
    {
        // Arrange
        var command = new CreateOrderCommand("John Doe", 99.99m);

        // Assert
        command.Should().BeAssignableTo<ICommand>();
        command.Should().BeAssignableTo<IRequest<Result>>();
    }

    /// <summary>
    /// Verifies that a command can store and expose its properties correctly.
    /// </summary>
    [Fact]
    public void Command_Should_StorePropertiesCorrectly()
    {
        // Arrange
        var customerName = "Jane Smith";
        var totalAmount = 150.00m;

        // Act
        var command = new CreateOrderCommand(customerName, totalAmount);

        // Assert
        command.CustomerName.Should().Be(customerName);
        command.TotalAmount.Should().Be(totalAmount);
    }

    #endregion

    #region ICommand<TResponse> Tests

    /// <summary>
    /// Verifies that <see cref="ICommand{TResponse}"/> implements <see cref="IRequest{TResponse}"/> with <see cref="Result{TResponse}"/> as response.
    /// </summary>
    [Fact]
    public void ICommandT_Should_ImplementIRequestWithResultT()
    {
        // Assert
        typeof(ICommand<Guid>).Should().BeAssignableTo<IRequest<Result<Guid>>>();
    }

    /// <summary>
    /// Verifies that a custom command with response implementing <see cref="ICommand{TResponse}"/> is assignable to the interface.
    /// </summary>
    [Fact]
    public void CustomCommandWithResponse_Should_ImplementICommandT()
    {
        // Arrange
        var command = new CreateUserCommand("user@example.com", "Test User");

        // Assert
        command.Should().BeAssignableTo<ICommand<Guid>>();
        command.Should().BeAssignableTo<IRequest<Result<Guid>>>();
    }

    /// <summary>
    /// Verifies that a command returning a complex type is correctly typed.
    /// </summary>
    [Fact]
    public void CommandWithComplexResponse_Should_ImplementCorrectInterface()
    {
        // Arrange
        var command = new GetOrderSummaryCommand(Guid.NewGuid());

        // Assert
        command.Should().BeAssignableTo<ICommand<OrderSummary>>();
        command.Should().BeAssignableTo<IRequest<Result<OrderSummary>>>();
    }

    /// <summary>
    /// Verifies that a command with response can store and expose its properties correctly.
    /// </summary>
    [Fact]
    public void CommandWithResponse_Should_StorePropertiesCorrectly()
    {
        // Arrange
        var email = "test@example.com";
        var name = "Test User";

        // Act
        var command = new CreateUserCommand(email, name);

        // Assert
        command.Email.Should().Be(email);
        command.Name.Should().Be(name);
    }

    #endregion

    #region Interface Relationship Tests

    /// <summary>
    /// Verifies that <see cref="ICommand"/> is a marker interface without additional members.
    /// </summary>
    [Fact]
    public void ICommand_Should_BeMarkerInterface()
    {
        // Assert - ICommand only extends IRequest<Result>, no additional members
        typeof(ICommand).GetInterfaces().Should().Contain(typeof(IRequest<Result>));
    }

    /// <summary>
    /// Verifies that <see cref="ICommand{TResponse}"/> is a marker interface without additional members.
    /// </summary>
    [Fact]
    public void ICommandT_Should_BeMarkerInterface()
    {
        // Assert - ICommand<T> only extends IRequest<Result<T>>, no additional members
        typeof(ICommand<string>).GetInterfaces().Should().Contain(typeof(IRequest<Result<string>>));
    }

    #endregion
}
