using VehicleSearchService.Domain.Events;

namespace VehicleSearchService.Infrastructure.Messaging;

/// <summary>In-process reaction to <see cref="VehicleReservedEvent"/>.</summary>
public interface IVehicleReservedEventHandler
{
    Task HandleAsync(VehicleReservedEvent domainEvent, CancellationToken cancellationToken);
}
