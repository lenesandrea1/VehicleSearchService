using Microsoft.EntityFrameworkCore;
using VehicleSearchService.Domain.Enums;
using VehicleSearchService.Infrastructure.Persistence.Entities;

namespace VehicleSearchService.Infrastructure.Persistence;

public static class PersistenceSeeding
{
    public static async Task SeedAsync(VssDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Locations.AnyAsync(cancellationToken).ConfigureAwait(false))
            return;

        const string marketEs = "EU-ES";
        const string marketFr = "EU-FR";

        var madrid = new LocationRecord
        {
            Id = KnownIds.LocationMadrid,
            MarketId = marketEs,
            Name = "Madrid Downtown"
        };

        var barcelona = new LocationRecord
        {
            Id = KnownIds.LocationBarcelona,
            MarketId = marketEs,
            Name = "Barcelona Airport"
        };

        var paris = new LocationRecord
        {
            Id = KnownIds.LocationParis,
            MarketId = marketFr,
            Name = "Paris City"
        };

        var economy = new VehicleRecord
        {
            Id = KnownIds.VehicleEconomy,
            LocationId = KnownIds.LocationMadrid,
            Status = VehicleStatus.Available,
            VehicleTypeCatalogId = "vt-economy"
        };
        economy.EnabledMarkets.Add(new VehicleMarketRecord { VehicleId = economy.Id, MarketCode = marketEs });

        var suv = new VehicleRecord
        {
            Id = KnownIds.VehicleSuv,
            LocationId = KnownIds.LocationMadrid,
            Status = VehicleStatus.Available,
            VehicleTypeCatalogId = "vt-suv"
        };
        suv.EnabledMarkets.Add(new VehicleMarketRecord { VehicleId = suv.Id, MarketCode = marketEs });

        var wrongMarket = new VehicleRecord
        {
            Id = KnownIds.VehicleWrongMarket,
            LocationId = KnownIds.LocationMadrid,
            Status = VehicleStatus.Available,
            VehicleTypeCatalogId = "vt-compact"
        };
        wrongMarket.EnabledMarkets.Add(new VehicleMarketRecord { VehicleId = wrongMarket.Id, MarketCode = marketFr });

        var outOfService = new VehicleRecord
        {
            Id = KnownIds.VehicleOutOfService,
            LocationId = KnownIds.LocationMadrid,
            Status = VehicleStatus.Unavailable,
            VehicleTypeCatalogId = "vt-van"
        };
        outOfService.EnabledMarkets.Add(new VehicleMarketRecord { VehicleId = outOfService.Id, MarketCode = marketEs });

        var parisCar = new VehicleRecord
        {
            Id = KnownIds.VehicleParis,
            LocationId = KnownIds.LocationParis,
            Status = VehicleStatus.Available,
            VehicleTypeCatalogId = "vt-suv"
        };
        parisCar.EnabledMarkets.Add(new VehicleMarketRecord { VehicleId = parisCar.Id, MarketCode = marketFr });

        var sampleReservation = new ReservationRecord
        {
            Id = KnownIds.ReservationSample,
            VehicleId = KnownIds.VehicleEconomy,
            PickupLocationId = KnownIds.LocationMadrid,
            ReturnLocationId = KnownIds.LocationBarcelona,
            PickupAtUtc = new DateTime(2026, 6, 10, 10, 0, 0, DateTimeKind.Utc),
            ReturnAtUtc = new DateTime(2026, 6, 20, 10, 0, 0, DateTimeKind.Utc),
            Status = ReservationStatus.Confirmed
        };

        db.Locations.AddRange(madrid, barcelona, paris);
        db.Vehicles.AddRange(economy, suv, wrongMarket, outOfService, parisCar);
        db.Reservations.Add(sampleReservation);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
