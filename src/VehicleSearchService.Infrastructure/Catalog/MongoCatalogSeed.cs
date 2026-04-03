using MongoDB.Driver;

namespace VehicleSearchService.Infrastructure.Catalog;

/// <summary>Inserts sample catalog rows if collections are empty (aligned with relational seed).</summary>
public static class MongoCatalogSeed
{
    public static async Task EnsureAsync(IMongoDatabase database, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(database);

        var markets = database.GetCollection<MarketDocument>("markets");
        if (await markets.CountDocumentsAsync(FilterDefinition<MarketDocument>.Empty, cancellationToken: cancellationToken)
                .ConfigureAwait(false) > 0)
            return;

        var vehicleTypes = database.GetCollection<VehicleTypeDocument>("vehicle_types");

        await markets
            .InsertOneAsync(
                new MarketDocument { Id = "EU-ES", Name = "Spain" },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await vehicleTypes
            .InsertManyAsync(
                new[]
                {
                    new VehicleTypeDocument { Id = "vt-economy", Name = "Economy" },
                    new VehicleTypeDocument { Id = "vt-suv", Name = "SUV" }
                },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
