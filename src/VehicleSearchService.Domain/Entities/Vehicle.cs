using VehicleSearchService.Domain.Enums;

namespace VehicleSearchService.Domain.Entities;

public sealed class Vehicle
{
    public required Guid Id { get; init; }

    /// <summary>Station where the vehicle is available for pickup.</summary>
    public required Guid LocationId { get; init; }

    public required VehicleStatus Status { get; init; }

    /// <summary>Optional catalog key for vehicle type (e.g. MongoDB document id).</summary>
    public string? VehicleTypeCatalogId { get; init; }

    /// <summary>Markets where this vehicle may operate; must include the pickup location's market.</summary>
    public required IReadOnlySet<string> EnabledMarketIds { get; init; }

    public bool IsListedAsAvailable => Status == VehicleStatus.Available;

    public bool IsAtLocation(Guid locationId) => LocationId == locationId;

    public bool IsEnabledForMarket(string marketId) => EnabledMarketIds.Contains(marketId);

    /// <summary>Pickup station and market rules only (does not evaluate reservations).</summary>
    public bool SatisfiesPickupStationAndMarket(Location pickupLocation)
    {
        ArgumentNullException.ThrowIfNull(pickupLocation);
        return IsListedAsAvailable
            && IsAtLocation(pickupLocation.Id)
            && IsEnabledForMarket(pickupLocation.MarketId);
    }

    /// <summary>
    /// Whether the vehicle may appear in search results for the requested window.
    /// <paramref name="reservations"/> must contain reservations for this vehicle only.
    /// </summary>
    public bool CanBeOfferedInSearch(
        Location pickupLocation,
        DateTime pickupUtc,
        DateTime returnUtc,
        IEnumerable<Reservation> reservations)
    {
        ArgumentNullException.ThrowIfNull(pickupLocation);
        ArgumentNullException.ThrowIfNull(reservations);
        if (!SatisfiesPickupStationAndMarket(pickupLocation))
            return false;

        foreach (var reservation in reservations)
        {
            if (reservation.ConflictsWithRentalRequest(Id, pickupUtc, returnUtc))
                return false;
        }

        return true;
    }
}
