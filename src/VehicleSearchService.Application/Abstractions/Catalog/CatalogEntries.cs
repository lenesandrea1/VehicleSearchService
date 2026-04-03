namespace VehicleSearchService.Application.Abstractions.Catalog;

public sealed record MarketCatalogEntry(string Id, string DisplayName);

public sealed record VehicleTypeCatalogEntry(string Id, string DisplayName);
