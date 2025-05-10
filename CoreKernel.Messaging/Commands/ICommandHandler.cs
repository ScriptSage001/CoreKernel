using CoreKernel.Functional.Results;
using MediatR;

namespace CoreKernel.Messaging.Commands;

/// <summary>
/// Marker interface for the command handlers.
/// Implements the MediatR <see cref="IRequestHandler{TRequest, TResponse}"/> interface
/// to handle commands that do not return a response.
/// </summary>
/// <typeparam name="TCommand">
/// The type of the command being handled. Must implement the <see cref="ICommand"/> interface.
/// </typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

/// <summary>
/// Marker interface for the command handlers with a response.
/// Implements the MediatR <see cref="IRequestHandler{TRequest, TResponse}"/> interface
/// to handle commands that return a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TCommand">
/// The type of the command being handled. Must implement the <see cref="ICommand{TResponse}"/> interface.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the command handler.
/// </typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;