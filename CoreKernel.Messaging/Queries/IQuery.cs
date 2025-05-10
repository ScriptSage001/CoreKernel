using CoreKernel.Functional.Results;
using MediatR;

namespace CoreKernel.Messaging.Queries;

/// <summary>
/// Marker interface for the queries.
/// Represents a query that returns a result of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response returned by the query.
/// </typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;