namespace VehicleSearchService.Domain.Common;

/// <summary>Domain event raised after a meaningful change to the model.</summary>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
