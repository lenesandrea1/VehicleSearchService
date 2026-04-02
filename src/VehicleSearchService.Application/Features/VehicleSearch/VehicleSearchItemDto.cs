namespace VehicleSearchService.Application.Features.VehicleSearch;

public sealed record VehicleSearchItemDto(
    Guid Id,
    Guid LocationId,
    string? VehicleTypeCatalogId);
