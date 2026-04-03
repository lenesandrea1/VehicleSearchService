namespace VehicleSearchService.Infrastructure.Persistence.Entities;

public sealed class LocationRecord
{
    public Guid Id { get; set; }
    public string MarketId { get; set; } = string.Empty;
    public string? Name { get; set; }
}
