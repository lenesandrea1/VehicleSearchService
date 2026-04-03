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

        var market = "EU-ES";

        var madrid = new LocationRecord
        {
            Id = KnownIds.LocationMadrid,
            MarketId = market,
            Name = "Madrid Downtown"
        };

        var barcelona = new LocationRecord
        {
            Id = KnownIds.LocationBarcelona,
            MarketId = market,
            Name = "Barcelona Airport"
        };

        var economy = new VehicleRecord
        {
            Id = KnownIds.VehicleEconomy,
            LocationId = KnownIds.LocationMadrid,
            Status = VehicleStatus.Available,
            VehicleTypeCatalogId = "vt-economy"
        };

        economy.EnabledMarkets.Add(new VehicleMarketRecord { VehicleId = economy.Id, MarketCode = market });

        var suv = new VehicleRecord
        {
            Id = KnownIds.VehicleSuv,
            LocationId = KnownIds.LocationMadrid,
            Status = VehicleStatus.Available,
            VehicleTypeCatalogId = "vt-suv"
        };

        suv.EnabledMarkets.Add(new VehicleMarketRecord { VehicleId = suv.Id, MarketCode = market });

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

        db.Locations.AddRange(madrid, barcelona);
        db.Vehicles.AddRange(economy, suv);
        db.Reservations.Add(sampleReservation);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
