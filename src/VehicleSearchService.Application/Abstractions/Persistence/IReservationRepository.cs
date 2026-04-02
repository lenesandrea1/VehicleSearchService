using VehicleSearchService.Domain.Entities;

namespace VehicleSearchService.Application.Abstractions.Persistence;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> ListPotentiallyBlockingAsync(
        IReadOnlyCollection<Guid> vehicleIds,
        DateTime pickupUtc,
        DateTime returnUtc,
        CancellationToken cancellationToken = default);

    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
}
