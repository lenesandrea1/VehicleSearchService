using Moq;
using VehicleSearchService.Application.Abstractions.Catalog;
using VehicleSearchService.Application.Abstractions.Persistence;
using VehicleSearchService.Application.Features.VehicleSearch;
using VehicleSearchService.Domain.Entities;
using VehicleSearchService.Domain.Enums;

namespace VehicleSearchService.Tests.Unit.Application;

public sealed class SearchVehiclesQueryHandlerTests
{
    [Fact]
    public async Task Enriches_results_with_catalog_display_names()
    {
        var pickupId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        const string market = "EU-XX";
        var pickup = new Location
        {
            Id = pickupId,
            MarketId = market,
            Name = "City A"
        };
        var vehicle = new Vehicle
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
            LocationId = pickupId,
            Status = VehicleStatus.Available,
            VehicleTypeCatalogId = "vt-test",
            EnabledMarketIds = new HashSet<string>(StringComparer.Ordinal) { market }
        };
        var pickupAt = new DateTime(2026, 3, 1, 9, 0, 0, DateTimeKind.Utc);
        var returnAt = new DateTime(2026, 3, 5, 9, 0, 0, DateTimeKind.Utc);

        var locations = new Mock<ILocationReadRepository>();
        locations
            .Setup(l => l.GetByIdAsync(pickupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pickup);

        var vehicles = new Mock<IVehicleReadRepository>();
        vehicles
            .Setup(v => v.ListCandidatesAtLocationAsync(pickupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });

        var reservations = new Mock<IReservationRepository>();
        reservations
            .Setup(r => r.ListPotentiallyBlockingAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                pickupAt,
                returnAt,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Reservation>);

        var catalog = new Mock<ICatalogReader>();
        catalog
            .Setup(c => c.GetMarketAsync(market, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MarketCatalogEntry(market, "Test market"));
        catalog
            .Setup(c => c.GetVehicleTypesAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new Dictionary<string, VehicleTypeCatalogEntry>(StringComparer.Ordinal)
                {
                    ["vt-test"] = new VehicleTypeCatalogEntry("vt-test", "Compact")
                });

        var handler = new SearchVehiclesQueryHandler(
            locations.Object,
            vehicles.Object,
            reservations.Object,
            catalog.Object);

        var result = await handler.HandleAsync(new SearchVehiclesQuery(pickupId, pickupId, pickupAt, returnAt));

        Assert.Equal(market, result.PickupMarketId);
        Assert.Equal("Test market", result.PickupMarketDisplayName);
        var item = Assert.Single(result.Items);
        Assert.Equal(vehicle.Id, item.Id);
        Assert.Equal("Compact", item.VehicleTypeDisplayName);
    }
}
