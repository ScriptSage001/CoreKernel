using CoreKernel.Functional.Results;
using MediatR;

namespace CoreKernel.Messaging.Queries;

/// <summary>
/// Marker interface for the query handlers.
/// Implements the MediatR <see cref="IRequestHandler{TRequest, TResponse}"/> interface
/// to handle queries that return a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">
/// The type of the query being handled. Must implement the <see cref="IQuery{TResponse}"/> interface.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the query handler.
/// </typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;