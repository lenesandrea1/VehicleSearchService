namespace VehicleSearchService.Application.Features.VehicleSearch;

public sealed record SearchVehiclesResult(IReadOnlyList<VehicleSearchItemDto> Items);
