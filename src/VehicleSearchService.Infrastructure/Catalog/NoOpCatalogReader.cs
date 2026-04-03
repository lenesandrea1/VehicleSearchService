using VehicleSearchService.Application.Abstractions.Catalog;

namespace VehicleSearchService.Infrastructure.Catalog;

/// <summary>Used when catalog integration is turned off (e.g. lightweight integration tests).</summary>
public sealed class NoOpCatalogReader : ICatalogReader
{
    public Task<MarketCatalogEntry?> GetMarketAsync(string marketId, CancellationToken cancellationToken = default) =>
        Task.FromResult<MarketCatalogEntry?>(null);

    public Task<IReadOnlyDictionary<string, VehicleTypeCatalogEntry>> GetVehicleTypesAsync(
        IReadOnlyCollection<string> catalogIds,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyDictionary<string, VehicleTypeCatalogEntry>>(
            new Dictionary<string, VehicleTypeCatalogEntry>(StringComparer.Ordinal));
}
