using Microsoft.Extensions.Logging;
using VehicleSearchService.Application.Abstractions.Messaging;
using VehicleSearchService.Domain.Common;
using VehicleSearchService.Domain.Events;

namespace VehicleSearchService.Infrastructure.Messaging;

public sealed class InMemoryDomainEventPublisher(
    IEnumerable<IVehicleReservedEventHandler> vehicleReservedHandlers,
    ILogger<InMemoryDomainEventPublisher> logger) : IDomainEventPublisher
{
    public async Task PublishAsync(IReadOnlyCollection<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var e in events)
        {
            if (e is VehicleReservedEvent v)
            {
                foreach (var handler in vehicleReservedHandlers)
                {
                    try
                    {
                        await handler.HandleAsync(v, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(
                            ex,
                            "Domain event handler {HandlerType} failed for reservation {ReservationId}; reservation persistence is already committed.",
                            handler.GetType().Name,
                            v.ReservationId);
                    }
                }
            }
            else
            {
                logger.LogWarning("No in-memory handler registered for {EventType}", e.GetType().Name);
            }
        }
    }
}
