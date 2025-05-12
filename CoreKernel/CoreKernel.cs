// Global using directives for commonly used namespaces in the CoreKernel project.
// These directives make the specified namespaces available throughout the project
// without requiring explicit `using` statements in each file.

global using CoreKernel.Primitives.Entities; // Provides base classes for entities and aggregate roots.
global using CoreKernel.Primitives.ValueObjects; // Provides base classes for value objects.
global using CoreKernel.Primitives.Abstractions; // Contains abstractions for domain primitives.

global using CoreKernel.Functional.Maybe; // Provides support for the Maybe monad for optional values.
global using CoreKernel.Functional.Results; // Provides support for functional result types.
global using CoreKernel.Functional.Validation; // Provides validation utilities for functional programming.
global using CoreKernel.Functional.Extensions; // Provides extension methods for functional programming.

global using CoreKernel.DomainMarkers.Auditing; // Contains domain markers for auditing entities.
global using CoreKernel.DomainMarkers.MultiTenancy; // Contains domain markers for multi-tenancy support.
global using CoreKernel.DomainMarkers.SoftDeletion; // Contains domain markers for soft deletion functionality.
global using CoreKernel.DomainMarkers.Tracing; // Contains domain markers for tracing operations.

global using CoreKernel.Messaging.Commands; // Provides support for command messaging in the application.
global using CoreKernel.Messaging.Queries; // Provides support for query messaging in the application.
global using CoreKernel.Messaging.Events; // Provides support for event messaging in the application.