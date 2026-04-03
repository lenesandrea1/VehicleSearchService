namespace VehicleSearchService.Application.Common.Exceptions;

public sealed class VehicleNotFoundException : Exception
{
    public VehicleNotFoundException(Guid vehicleId)
        : base($"Vehicle '{vehicleId}' was not found.") { }
}
