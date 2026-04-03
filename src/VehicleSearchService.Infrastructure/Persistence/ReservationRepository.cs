using Microsoft.EntityFrameworkCore;
using VehicleSearchService.Application.Abstractions.Persistence;
using VehicleSearchService.Domain.Entities;
using VehicleSearchService.Domain.Enums;
using VehicleSearchService.Infrastructure.Persistence.Entities;

namespace VehicleSearchService.Infrastructure.Persistence;

public sealed class ReservationRepository(VssDbContext db) : IReservationRepository
{
    public async Task<IReadOnlyList<Reservation>> ListPotentiallyBlockingAsync(
        IReadOnlyCollection<Guid> vehicleIds,
        DateTime pickupUtc,
        DateTime returnUtc,
        CancellationToken cancellationToken = default)
    {
        if (vehicleIds.Count == 0)
            return [];

        var rows = await db.Reservations
            .AsNoTracking()
            .Where(r => vehicleIds.Contains(r.VehicleId))
            .Where(r => r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed)
            .Where(r => r.PickupAtUtc < returnUtc && pickupUtc < r.ReturnAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rows.ConvertAll(Map);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        var entity = new ReservationRecord
        {
            Id = reservation.Id,
            VehicleId = reservation.VehicleId,
            PickupLocationId = reservation.PickupLocationId,
            ReturnLocationId = reservation.ReturnLocationId,
            PickupAtUtc = reservation.PickupAtUtc,
            ReturnAtUtc = reservation.ReturnAtUtc,
            Status = reservation.Status
        };

        db.Reservations.Add(entity);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Reservation Map(ReservationRecord r) =>
        Reservation.Rehydrate(
            r.Id,
            r.VehicleId,
            r.PickupLocationId,
            r.ReturnLocationId,
            r.PickupAtUtc,
            r.ReturnAtUtc,
            r.Status);
}
