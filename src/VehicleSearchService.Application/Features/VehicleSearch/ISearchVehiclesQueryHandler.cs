namespace VehicleSearchService.Application.Features.VehicleSearch;

public interface ISearchVehiclesQueryHandler
{
    Task<SearchVehiclesResult> HandleAsync(SearchVehiclesQuery query, CancellationToken cancellationToken = default);
}
