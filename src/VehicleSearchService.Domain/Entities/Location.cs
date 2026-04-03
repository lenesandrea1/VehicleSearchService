namespace VehicleSearchService.Domain.Entities;

/// <summary>Pickup or return station; <see cref="MarketId"/> references catalog data (e.g. MongoDB).</summary>
public sealed class Location
{
    public required Guid Id { get; init; }

    /// <summary>Market / country identifier in the catalog.</summary>
    public required string MarketId { get; init; }

    public string? Name { get; init; }
}
