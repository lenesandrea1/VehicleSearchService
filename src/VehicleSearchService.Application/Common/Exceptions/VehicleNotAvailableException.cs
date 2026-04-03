namespace VehicleSearchService.Application.Common.Exceptions;

public sealed class VehicleNotAvailableException : Exception
{
    public VehicleNotAvailableException(string message) : base(message) { }
}
