namespace VehicleSearchService.Application.Common.Exceptions;

public sealed class ReturnLocationNotFoundException : Exception
{
    public ReturnLocationNotFoundException(Guid locationId)
        : base($"Return location '{locationId}' was not found.") { }
}
