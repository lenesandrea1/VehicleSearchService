namespace VehicleSearchService.Application.Common.Exceptions;

public sealed class PickupLocationNotFoundException : Exception
{
    public PickupLocationNotFoundException(Guid locationId)
        : base($"Pickup location '{locationId}' was not found.") { }
}
