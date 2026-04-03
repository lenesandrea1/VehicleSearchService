using VehicleSearchService.Application.Abstractions.Catalog;
using VehicleSearchService.Application.Abstractions.Persistence;
using VehicleSearchService.Application.Common.Validation;

namespace VehicleSearchService.Application.Features.VehicleSearch;

public sealed class SearchVehiclesQueryHandler(
    ILocationReadRepository locations,
    IVehicleReadRepository vehicles,
    IReservationRepository reservations,
    ICatalogReader catalog,
    TimeProvider time) : ISearchVehiclesQueryHandler
{
    public async Task<SearchVehiclesResult> HandleAsync(SearchVehiclesQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var utcNow = time.GetUtcNow().UtcDateTime;
        RentalTimeWindowGuard.EnsureValidForRequest(query.PickupAtUtc, query.ReturnAtUtc, utcNow);

        var pickupLocation = await locations
            .GetByIdAsync(query.PickupLocationId, cancellationToken)
            .ConfigureAwait(false);

        if (pickupLocation is null)
            return new SearchVehiclesResult(Array.Empty<VehicleSearchItemDto>(), null, null);

        var candidates = await vehicles
            .ListCandidatesAtLocationAsync(query.PickupLocationId, cancellationToken)
            .ConfigureAwait(false);

        if (candidates.Count == 0)
        {
            var onlyMarket = await catalog
                .GetMarketAsync(pickupLocation.MarketId, cancellationToken)
                .ConfigureAwait(false);
            return new SearchVehiclesResult(
                Array.Empty<VehicleSearchItemDto>(),
                pickupLocation.MarketId,
                onlyMarket?.DisplayName);
        }

        var vehicleIds = candidates.Select(v => v.Id).ToArray();
        var blocking = await reservations
            .ListPotentiallyBlockingAsync(vehicleIds, query.PickupAtUtc, query.ReturnAtUtc, cancellationToken)
            .ConfigureAwait(false);

        var byVehicle = blocking.GroupBy(r => r.VehicleId).ToDictionary(g => g.Key, g => g.ToList());

        var items = new List<VehicleSearchItemDto>();
        foreach (var vehicle in candidates)
        {
            byVehicle.TryGetValue(vehicle.Id, out var list);
            list ??= [];

            if (!vehicle.CanBeOfferedInSearch(pickupLocation, query.PickupAtUtc, query.ReturnAtUtc, list))
                continue;

            items.Add(new VehicleSearchItemDto(vehicle.Id, vehicle.LocationId, vehicle.VehicleTypeCatalogId, null));
        }

        if (items.Count == 0)
        {
            var m = await catalog.GetMarketAsync(pickupLocation.MarketId, cancellationToken).ConfigureAwait(false);
            return new SearchVehiclesResult(items, pickupLocation.MarketId, m?.DisplayName);
        }

        var distinctTypeIds = items
            .Select(i => i.VehicleTypeCatalogId)
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .Select(static id => id!)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var marketTask = catalog.GetMarketAsync(pickupLocation.MarketId, cancellationToken);
        var typesTask = catalog.GetVehicleTypesAsync(distinctTypeIds, cancellationToken);
        await Task.WhenAll(marketTask, typesTask).ConfigureAwait(false);

        var marketEntry = await marketTask.ConfigureAwait(false);
        var typeMap = await typesTask.ConfigureAwait(false);

        var withLabels = items.ConvertAll(dto =>
        {
            string? label = null;
            if (dto.VehicleTypeCatalogId is { } cid && typeMap.TryGetValue(cid, out var entry))
                label = entry.DisplayName;

            return dto with { VehicleTypeDisplayName = label };
        });

        return new SearchVehiclesResult(withLabels, pickupLocation.MarketId, marketEntry?.DisplayName);
    }
}
