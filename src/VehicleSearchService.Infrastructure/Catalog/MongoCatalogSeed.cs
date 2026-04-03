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
            .InsertManyAsync(
                new[]
                {
                    new MarketDocument { Id = "EU-ES", Name = "Spain" },
                    new MarketDocument { Id = "EU-FR", Name = "France" }
                },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await vehicleTypes
            .InsertManyAsync(
                new[]
                {
                    new VehicleTypeDocument { Id = "vt-economy", Name = "Economy" },
                    new VehicleTypeDocument { Id = "vt-suv", Name = "SUV" },
                    new VehicleTypeDocument { Id = "vt-compact", Name = "Compact" },
                    new VehicleTypeDocument { Id = "vt-van", Name = "Van" }
                },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
