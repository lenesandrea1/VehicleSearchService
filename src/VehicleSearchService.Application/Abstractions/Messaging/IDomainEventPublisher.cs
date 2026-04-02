using VehicleSearchService.Domain.Common;

namespace VehicleSearchService.Application.Abstractions.Messaging;

public interface IDomainEventPublisher
{
    Task PublishAsync(IReadOnlyCollection<IDomainEvent> events, CancellationToken cancellationToken = default);
}
