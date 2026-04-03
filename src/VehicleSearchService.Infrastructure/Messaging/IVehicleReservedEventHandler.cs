using VehicleSearchService.Domain.Events;

namespace VehicleSearchService.Infrastructure.Messaging;

public interface IVehicleReservedEventHandler
{
    Task HandleAsync(VehicleReservedEvent domainEvent, CancellationToken cancellationToken);
}
