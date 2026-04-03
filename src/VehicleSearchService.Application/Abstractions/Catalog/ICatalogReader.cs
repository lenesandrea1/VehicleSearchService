namespace VehicleSearchService.Application.Abstractions.Catalog;

/// <summary>Read-only catalog (MongoDB in production; stubbed when disabled).</summary>
public interface ICatalogReader
{
    Task<MarketCatalogEntry?> GetMarketAsync(string marketId, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, VehicleTypeCatalogEntry>> GetVehicleTypesAsync(
        IReadOnlyCollection<string> catalogIds,
        CancellationToken cancellationToken = default);
}
