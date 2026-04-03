namespace VehicleSearchService.Application.Abstractions.Catalog;

/// <summary>Market identifier and display label from catalog storage.</summary>
public sealed record MarketCatalogEntry(string Id, string DisplayName);

/// <summary>Vehicle type identifier and display label from catalog storage.</summary>
public sealed record VehicleTypeCatalogEntry(string Id, string DisplayName);
