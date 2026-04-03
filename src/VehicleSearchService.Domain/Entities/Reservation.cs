using VehicleSearchService.Domain.Common;
using VehicleSearchService.Domain.Enums;
using VehicleSearchService.Domain.Exceptions;

namespace VehicleSearchService.Domain.Entities;

public sealed class Reservation
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid PickupLocationId { get; private set; }
    public Guid ReturnLocationId { get; private set; }
    public DateTime PickupAtUtc { get; private set; }
    public DateTime ReturnAtUtc { get; private set; }
    public ReservationStatus Status { get; private set; }

    private Reservation() { }

    public static Reservation Create(
        Guid vehicleId,
        Guid pickupLocationId,
        Guid returnLocationId,
        DateTime pickupAtUtc,
        DateTime returnAtUtc,
        ReservationStatus initialStatus = ReservationStatus.Confirmed)
    {
        if (returnAtUtc <= pickupAtUtc)
            throw new DomainException("Return date and time must be after pickup date and time.");

        return new Reservation
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            PickupLocationId = pickupLocationId,
            ReturnLocationId = returnLocationId,
            PickupAtUtc = pickupAtUtc,
            ReturnAtUtc = returnAtUtc,
            Status = initialStatus
        };
    }

    /// <summary>Rehydrates a reservation from persistence (same invariants as stored row).</summary>
    public static Reservation Rehydrate(
        Guid id,
        Guid vehicleId,
        Guid pickupLocationId,
        Guid returnLocationId,
        DateTime pickupAtUtc,
        DateTime returnAtUtc,
        ReservationStatus status) =>
        new()
        {
            Id = id,
            VehicleId = vehicleId,
            PickupLocationId = pickupLocationId,
            ReturnLocationId = returnLocationId,
            PickupAtUtc = pickupAtUtc,
            ReturnAtUtc = returnAtUtc,
            Status = status
        };

    /// <summary>Pending or confirmed reservations still block availability.</summary>
    public bool IsBlockingAvailability =>
        Status is ReservationStatus.Pending or ReservationStatus.Confirmed;

    /// <summary>Whether this reservation blocks <paramref name="vehicleId"/> for <c>[pickupUtc, returnUtc)</c>.</summary>
    public bool ConflictsWithRentalRequest(Guid vehicleId, DateTime pickupUtc, DateTime returnUtc) =>
        VehicleId == vehicleId
        && IsBlockingAvailability
        && RentalPeriod.Overlaps(PickupAtUtc, ReturnAtUtc, pickupUtc, returnUtc);

    public void Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
            return;
        if (Status == ReservationStatus.Completed)
            throw new DomainException("Cannot cancel a reservation that is already completed.");
        Status = ReservationStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status == ReservationStatus.Cancelled)
            throw new DomainException("Cannot complete a cancelled reservation.");
        if (Status == ReservationStatus.Completed)
            return;
        Status = ReservationStatus.Completed;
    }
}
