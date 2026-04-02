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
            throw new DomainException("La fecha y hora de devolución deben ser posteriores a la de recogida.");

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

    /// <summary>
    /// Reserva que aún bloquea disponibilidad: pendiente o confirmada (no cancelada ni completada).
    /// </summary>
    public bool IsBlockingAvailability =>
        Status is ReservationStatus.Pending or ReservationStatus.Confirmed;

    /// <summary>
    /// Indica si esta reserva bloquea al vehículo indicado en el periodo [pickupUtc, returnUtc).
    /// </summary>
    public bool ConflictsWithRentalRequest(Guid vehicleId, DateTime pickupUtc, DateTime returnUtc) =>
        VehicleId == vehicleId
        && IsBlockingAvailability
        && RentalPeriod.Overlaps(PickupAtUtc, ReturnAtUtc, pickupUtc, returnUtc);

    public void Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
            return;
        if (Status == ReservationStatus.Completed)
            throw new DomainException("No se puede cancelar una reserva ya completada.");
        Status = ReservationStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status == ReservationStatus.Cancelled)
            throw new DomainException("No se puede completar una reserva cancelada.");
        if (Status == ReservationStatus.Completed)
            return;
        Status = ReservationStatus.Completed;
    }
}
