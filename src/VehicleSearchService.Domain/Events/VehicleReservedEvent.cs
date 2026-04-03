using VehicleSearchService.Domain.Common;

namespace VehicleSearchService.Domain.Events;

/// <summary>Raised when a reservation is created (confirmed or pending, depending on application policy).</summary>
public sealed class VehicleReservedEvent : IDomainEvent
{
    public VehicleReservedEvent(
        Guid reservationId,
        Guid vehicleId,
        Guid pickupLocationId,
        Guid returnLocationId,
        DateTime pickupAtUtc,
        DateTime returnAtUtc,
        DateTime occurredOnUtc)
    {
        ReservationId = reservationId;
        VehicleId = vehicleId;
        PickupLocationId = pickupLocationId;
        ReturnLocationId = returnLocationId;
        PickupAtUtc = pickupAtUtc;
        ReturnAtUtc = returnAtUtc;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid ReservationId { get; }
    public Guid VehicleId { get; }
    public Guid PickupLocationId { get; }
    public Guid ReturnLocationId { get; }
    public DateTime PickupAtUtc { get; }
    public DateTime ReturnAtUtc { get; }
    public DateTime OccurredOnUtc { get; }
}
