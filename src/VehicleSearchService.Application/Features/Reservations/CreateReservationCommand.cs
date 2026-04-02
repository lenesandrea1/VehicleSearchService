namespace VehicleSearchService.Application.Features.Reservations;

public sealed record CreateReservationCommand(
    Guid VehicleId,
    Guid PickupLocationId,
    Guid ReturnLocationId,
    DateTime PickupAtUtc,
    DateTime ReturnAtUtc);
