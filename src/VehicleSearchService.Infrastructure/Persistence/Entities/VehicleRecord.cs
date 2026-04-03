using VehicleSearchService.Domain.Enums;

namespace VehicleSearchService.Infrastructure.Persistence.Entities;

public sealed class VehicleRecord
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public VehicleStatus Status { get; set; }
    public string? VehicleTypeCatalogId { get; set; }

    public LocationRecord? Location { get; set; }
    public ICollection<VehicleMarketRecord> EnabledMarkets { get; set; } = new List<VehicleMarketRecord>();
}
