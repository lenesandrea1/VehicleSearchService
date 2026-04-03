using VehicleSearchService.Domain.Entities;

namespace VehicleSearchService.Application.Abstractions.Persistence;

/// <summary>Reservation persistence (blocking queries and inserts).</summary>
public interface IReservationRepository
{
    /// <summary>Reservations that may block availability for the given vehicles and UTC window.</summary>
    Task<IReadOnlyList<Reservation>> ListPotentiallyBlockingAsync(
        IReadOnlyCollection<Guid> vehicleIds,
        DateTime pickupUtc,
        DateTime returnUtc,
        CancellationToken cancellationToken = default);

    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
}
