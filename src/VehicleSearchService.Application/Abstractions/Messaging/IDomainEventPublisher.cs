using VehicleSearchService.Domain.Common;

namespace VehicleSearchService.Application.Abstractions.Messaging;

/// <summary>Publishes domain events to in-process handlers (no message bus).</summary>
public interface IDomainEventPublisher
{
    Task PublishAsync(IReadOnlyCollection<IDomainEvent> events, CancellationToken cancellationToken = default);
}
