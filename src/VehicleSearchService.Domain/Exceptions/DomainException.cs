namespace VehicleSearchService.Domain.Exceptions;

/// <summary>Thrown when a domain invariant is violated.</summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
