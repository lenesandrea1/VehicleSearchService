namespace VehicleSearchService.Infrastructure.Persistence.Entities;

public sealed class VehicleMarketRecord
{
    public Guid VehicleId { get; set; }
    public string MarketCode { get; set; } = string.Empty;

    public VehicleRecord? Vehicle { get; set; }
}
