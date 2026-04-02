namespace VehicleSearchService.Application.Features.VehicleSearch;

public sealed record SearchVehiclesQuery(
    Guid PickupLocationId,
    Guid ReturnLocationId,
    DateTime PickupAtUtc,
    DateTime ReturnAtUtc);
