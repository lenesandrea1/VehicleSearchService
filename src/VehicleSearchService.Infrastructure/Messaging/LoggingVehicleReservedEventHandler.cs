using Microsoft.Extensions.Logging;
using VehicleSearchService.Domain.Events;

namespace VehicleSearchService.Infrastructure.Messaging;

public sealed class LoggingVehicleReservedEventHandler(ILogger<LoggingVehicleReservedEventHandler> logger)
    : IVehicleReservedEventHandler
{
    public Task HandleAsync(VehicleReservedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Vehicle {VehicleId} reserved. Reservation {ReservationId}, pickup {PickupUtc}, return {ReturnUtc}",
            domainEvent.VehicleId,
            domainEvent.ReservationId,
            domainEvent.PickupAtUtc,
            domainEvent.ReturnAtUtc);
        return Task.CompletedTask;
    }
}
