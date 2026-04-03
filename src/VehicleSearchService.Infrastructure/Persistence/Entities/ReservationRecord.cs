using VehicleSearchService.Domain.Enums;

namespace VehicleSearchService.Infrastructure.Persistence.Entities;

public sealed class ReservationRecord
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid PickupLocationId { get; set; }
    public Guid ReturnLocationId { get; set; }
    public DateTime PickupAtUtc { get; set; }
    public DateTime ReturnAtUtc { get; set; }
    public ReservationStatus Status { get; set; }
}
