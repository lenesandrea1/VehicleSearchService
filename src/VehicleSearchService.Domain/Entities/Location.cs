namespace VehicleSearchService.Domain.Entities;

/// <summary>Sede de recogida o devolución; <see cref="MarketId"/> enlaza con el catálogo (p. ej. Mongo).</summary>
public sealed class Location
{
    public required Guid Id { get; init; }

    /// <summary>Identificador del mercado/país en catálogo.</summary>
    public required string MarketId { get; init; }

    public string? Name { get; init; }
}
