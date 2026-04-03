namespace VehicleSearchService.Application.Common.Exceptions;

public sealed class InvalidReservationPeriodException : Exception
{
    public InvalidReservationPeriodException(string message) : base(message) { }
}
