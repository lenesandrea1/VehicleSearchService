using VehicleSearchService.Domain.Enums;

namespace VehicleSearchService.Domain.Entities;

public sealed class Vehicle
{
    public required Guid Id { get; init; }

    /// <summary>Localidad donde el vehículo está disponible para recogida.</summary>
    public required Guid LocationId { get; init; }

    public required VehicleStatus Status { get; init; }

    /// <summary>Mercados (países) en los que el vehículo puede operar; debe incluir el mercado de la localidad solicitada.</summary>
    public required IReadOnlySet<string> EnabledMarketIds { get; init; }

    public bool IsListedAsAvailable => Status == VehicleStatus.Available;

    public bool IsAtLocation(Guid locationId) => LocationId == locationId;

    public bool IsEnabledForMarket(string marketId) => EnabledMarketIds.Contains(marketId);

    /// <summary>
    /// Reglas de estación y mercado para la localidad de recogida (sin comprobar reservas).
    /// </summary>
    public bool SatisfiesPickupStationAndMarket(Location pickupLocation)
    {
        ArgumentNullException.ThrowIfNull(pickupLocation);
        return IsListedAsAvailable
            && IsAtLocation(pickupLocation.Id)
            && IsEnabledForMarket(pickupLocation.MarketId);
    }

    /// <summary>
    /// Determina si el vehículo puede ofertarse en búsqueda para el periodo dado.
    /// <paramref name="reservations"/> debe contener solo reservas del mismo vehículo.
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
