namespace VehicleSearchService.Application.Common.Exceptions;

/// <summary>Pickup/return instants violate ordering or a pickup-in-the-past rule.</summary>
public sealed class InvalidRentalTimeWindowException : Exception
{
    public InvalidRentalTimeWindowException(string message) : base(message) { }
}
