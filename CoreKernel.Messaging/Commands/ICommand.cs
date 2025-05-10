using CoreKernel.Functional.Results;
using MediatR;

namespace CoreKernel.Messaging.Commands;

/// <summary>
/// Marker interface for the commands in the application.
/// Represents a command that does not return a specific response but produces a <see cref="Result"/>.
/// </summary>
public interface ICommand : IRequest<Result>;

/// <summary>
/// Marker interface for the commands with a response.
/// Represents a command that returns a <see cref="Result{TResponse}"/> containing a specific response type.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;