using MongoDB.Driver;
using VehicleSearchService.Application.Abstractions.Catalog;

namespace VehicleSearchService.Infrastructure.Catalog;

internal sealed class MongoCatalogReader : ICatalogReader
{
    private readonly IMongoCollection<MarketDocument> _markets;
    private readonly IMongoCollection<VehicleTypeDocument> _vehicleTypes;

    public MongoCatalogReader(IMongoDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _markets = database.GetCollection<MarketDocument>("markets");
        _vehicleTypes = database.GetCollection<VehicleTypeDocument>("vehicle_types");
    }

    public async Task<MarketCatalogEntry?> GetMarketAsync(string marketId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(marketId))
            return null;

        var doc = await _markets
            .Find(d => d.Id == marketId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return doc is null ? null : new MarketCatalogEntry(doc.Id, doc.Name);
    }

    public async Task<IReadOnlyDictionary<string, VehicleTypeCatalogEntry>> GetVehicleTypesAsync(
        IReadOnlyCollection<string> catalogIds,
        CancellationToken cancellationToken = default)
    {
        if (catalogIds.Count == 0)
            return new Dictionary<string, VehicleTypeCatalogEntry>(StringComparer.Ordinal);

        var idList = catalogIds
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToList();
        if (idList.Count == 0)
            return new Dictionary<string, VehicleTypeCatalogEntry>(StringComparer.Ordinal);

        var filter = Builders<VehicleTypeDocument>.Filter.In(d => d.Id, idList);
        var cursor = await _vehicleTypes
            .FindAsync(filter, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var list = await cursor.ToListAsync(cancellationToken).ConfigureAwait(false);

        return list.ToDictionary(d => d.Id, d => new VehicleTypeCatalogEntry(d.Id, d.Name), StringComparer.Ordinal);
    }
}
