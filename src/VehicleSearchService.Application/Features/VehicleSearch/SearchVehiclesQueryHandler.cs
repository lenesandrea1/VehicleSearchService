using VehicleSearchService.Application.Abstractions.Persistence;

namespace VehicleSearchService.Application.Features.VehicleSearch;

public sealed class SearchVehiclesQueryHandler(
    ILocationReadRepository locations,
    IVehicleReadRepository vehicles,
    IReservationRepository reservations) : ISearchVehiclesQueryHandler
{
    public async Task<SearchVehiclesResult> HandleAsync(SearchVehiclesQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var pickupLocation = await locations
            .GetByIdAsync(query.PickupLocationId, cancellationToken)
            .ConfigureAwait(false);

        if (pickupLocation is null)
            return new SearchVehiclesResult(Array.Empty<VehicleSearchItemDto>());

        var candidates = await vehicles
            .ListCandidatesAtLocationAsync(query.PickupLocationId, cancellationToken)
            .ConfigureAwait(false);

        if (candidates.Count == 0)
            return new SearchVehiclesResult(Array.Empty<VehicleSearchItemDto>());

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

            items.Add(new VehicleSearchItemDto(vehicle.Id, vehicle.LocationId, vehicle.VehicleTypeCatalogId));
        }

        return new SearchVehiclesResult(items);
    }
}
