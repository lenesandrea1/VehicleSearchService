using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using VehicleSearchService.Infrastructure.Persistence;

namespace VehicleSearchService.Tests.Integration.ApiHost;

[Collection(nameof(SqlAndMongoCollection))]
public sealed class VehiclesSearchEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly SqlAndMongoFixture _fixture;

    public VehiclesSearchEndpointTests(SqlAndMongoFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Search_returns_both_vehicles_outside_blocked_reservation_window_with_catalog_labels()
    {
        var client = _fixture.Factory.CreateClient();
        var url = SearchUrl(
            KnownIds.LocationMadrid,
            KnownIds.LocationBarcelona,
            "2026-05-01T10:00:00.000Z",
            "2026-05-05T10:00:00.000Z");

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<SearchResponse>(JsonOptions);
        Assert.NotNull(payload);
        Assert.Equal("EU-ES", payload.PickupMarketId);
        Assert.Equal("Spain", payload.PickupMarketDisplayName);
        Assert.Equal(2, payload.Items.Count);
        Assert.Contains(payload.Items, i => i.Id == KnownIds.VehicleEconomy && i.VehicleTypeDisplayName == "Economy");
        Assert.Contains(payload.Items, i => i.Id == KnownIds.VehicleSuv && i.VehicleTypeDisplayName == "SUV");
    }

    [Fact]
    public async Task Search_excludes_economy_when_dates_overlap_sample_reservation()
    {
        var client = _fixture.Factory.CreateClient();
        var url = SearchUrl(
            KnownIds.LocationMadrid,
            KnownIds.LocationBarcelona,
            "2026-06-15T10:00:00.000Z",
            "2026-06-17T10:00:00.000Z");

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<SearchResponse>(JsonOptions);
        Assert.NotNull(payload);
        Assert.Single(payload.Items);
        Assert.Equal(KnownIds.VehicleSuv, payload.Items[0].Id);
    }

    [Fact]
    public async Task Search_returns_400_when_pickup_is_in_the_past()
    {
        var client = _fixture.Factory.CreateClient();
        var url = SearchUrl(
            KnownIds.LocationMadrid,
            KnownIds.LocationBarcelona,
            "2020-01-01T10:00:00.000Z",
            "2020-01-05T10:00:00.000Z");

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_returns_400_when_return_is_not_after_pickup()
    {
        var client = _fixture.Factory.CreateClient();
        var url = SearchUrl(
            KnownIds.LocationMadrid,
            KnownIds.LocationBarcelona,
            "2026-06-05T10:00:00.000Z",
            "2026-06-05T10:00:00.000Z");

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_returns_empty_when_pickup_location_unknown()
    {
        var client = _fixture.Factory.CreateClient();
        var missing = Guid.Parse("F0000000-0000-0000-0000-000000000001");
        var url = SearchUrl(
            missing,
            KnownIds.LocationBarcelona,
            "2026-05-01T10:00:00.000Z",
            "2026-05-05T10:00:00.000Z");

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<SearchResponse>(JsonOptions);
        Assert.NotNull(payload);
        Assert.Empty(payload.Items);
        Assert.Null(payload.PickupMarketId);
    }

    private static string SearchUrl(Guid pickup, Guid dropoff, string pickupUtc, string returnUtc) =>
        $"/api/vehicles/search?pickupLocationId={pickup}&returnLocationId={dropoff}&pickupAtUtc={Uri.EscapeDataString(pickupUtc)}&returnAtUtc={Uri.EscapeDataString(returnUtc)}";

    private sealed class SearchResponse
    {
        public List<SearchItem> Items { get; set; } = [];

        public string? PickupMarketId { get; set; }

        public string? PickupMarketDisplayName { get; set; }
    }

    private sealed class SearchItem
    {
        public Guid Id { get; set; }

        public string? VehicleTypeDisplayName { get; set; }
    }
}
