using System.ComponentModel.DataAnnotations;

namespace VehicleSearchService.Api.Contracts;

public sealed class CreateReservationRequest
{
    [Required]
    public Guid VehicleId { get; set; }

    [Required]
    public Guid PickupLocationId { get; set; }

    [Required]
    public Guid ReturnLocationId { get; set; }

    [Required]
    public DateTime PickupAtUtc { get; set; }

    [Required]
    public DateTime ReturnAtUtc { get; set; }
}
