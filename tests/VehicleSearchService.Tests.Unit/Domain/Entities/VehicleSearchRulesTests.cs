using FluentAssertions;
using VehicleSearchService.Domain.Entities;
using VehicleSearchService.Domain.Enums;

namespace VehicleSearchService.Tests.Unit.Domain.Entities;

public sealed class VehicleSearchRulesTests
{
    private static Location CreateLocation(string marketId = "ES-MAD") =>
        new() { Id = Guid.NewGuid(), MarketId = marketId, Name = "Test" };

    private static Vehicle CreateVehicle(Guid locationId, string marketId, VehicleStatus status = VehicleStatus.Available) =>
        new()
        {
            Id = Guid.NewGuid(),
            LocationId = locationId,
            Status = status,
            EnabledMarketIds = new HashSet<string> { marketId }
        };

    [Fact]
    public void CanBeOfferedInSearch_requires_available_status()
    {
        var pickup = CreateLocation();
        var vehicle = CreateVehicle(pickup.Id, pickup.MarketId, VehicleStatus.Unavailable);
        var from = new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc);

        vehicle.CanBeOfferedInSearch(pickup, from, to, Array.Empty<Reservation>()).Should().BeFalse();
    }

    [Fact]
    public void CanBeOfferedInSearch_requires_vehicle_at_pickup_location()
    {
        var pickup = CreateLocation();
        var vehicle = CreateVehicle(Guid.NewGuid(), pickup.MarketId);
        var from = new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc);

        vehicle.CanBeOfferedInSearch(pickup, from, to, Array.Empty<Reservation>()).Should().BeFalse();
    }

    [Fact]
    public void CanBeOfferedInSearch_requires_market_enabled_for_location()
    {
        var pickup = CreateLocation("ES");
        var vehicle = CreateVehicle(pickup.Id, "PT");
        var from = new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc);

        vehicle.CanBeOfferedInSearch(pickup, from, to, Array.Empty<Reservation>()).Should().BeFalse();
    }

    [Fact]
    public void CanBeOfferedInSearch_false_when_blocking_reservation_overlaps()
    {
        var pickup = CreateLocation();
        var vehicle = CreateVehicle(pickup.Id, pickup.MarketId);
        var from = new DateTime(2026, 5, 10, 10, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 5, 20, 10, 0, 0, DateTimeKind.Utc);
        var existing = Reservation.Create(
            vehicle.Id,
            pickup.Id,
            pickup.Id,
            new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 5, 25, 10, 0, 0, DateTimeKind.Utc));

        vehicle.CanBeOfferedInSearch(pickup, from, to, new[] { existing }).Should().BeFalse();
    }

    [Fact]
    public void CanBeOfferedInSearch_true_when_rules_hold_and_no_overlap()
    {
        var pickup = CreateLocation();
        var vehicle = CreateVehicle(pickup.Id, pickup.MarketId);
        var from = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 6, 5, 10, 0, 0, DateTimeKind.Utc);
        var existing = Reservation.Create(
            vehicle.Id,
            pickup.Id,
            pickup.Id,
            new DateTime(2026, 6, 10, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 6, 15, 10, 0, 0, DateTimeKind.Utc));

        vehicle.CanBeOfferedInSearch(pickup, from, to, new[] { existing }).Should().BeTrue();
    }
}
