using VehicleSearchService.Domain.Entities;
using VehicleSearchService.Infrastructure.Persistence.Entities;

namespace VehicleSearchService.Infrastructure.Persistence.Mapping;

internal static class VehicleMapper
{
    public static Vehicle ToDomain(VehicleRecord record)
    {
        var markets = record.EnabledMarkets.Select(m => m.MarketCode).ToHashSet(StringComparer.Ordinal);
        return new Vehicle
        {
            Id = record.Id,
            LocationId = record.LocationId,
            Status = record.Status,
            VehicleTypeCatalogId = record.VehicleTypeCatalogId,
            EnabledMarketIds = markets
        };
    }
}
