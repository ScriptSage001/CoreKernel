using System;

namespace CoreKernel.DomainMarkers.Tracing;

/// <summary>
/// Interface for traceable entities.
/// Provides properties to track the correlation identifier, trace source,
/// and operation name used for tracing operations across systems.
/// </summary>
public interface ITraceable
{
    /// <summary>
    /// Gets or sets the CorrelationId to track the request.
    /// This is a unique identifier used for tracing operations across systems.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the source of the trace.
    /// This indicates the origin or system that generated the trace.
    /// </summary>
    string? TraceSource { get; set; }

    /// <summary>
    /// Gets or sets the name of the operation being traced.
    /// This provides context about the specific operation or process.
    /// </summary>
    string? OperationName { get; set; }
}