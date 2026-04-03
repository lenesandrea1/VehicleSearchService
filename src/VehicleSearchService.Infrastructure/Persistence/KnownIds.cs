namespace VehicleSearchService.Infrastructure.Persistence;

/// <summary>Stable identifiers for seed data (documented for local testing).</summary>
public static class KnownIds
{
    public static readonly Guid LocationMadrid = Guid.Parse("A0000001-0000-0000-0000-000000000001");
    public static readonly Guid LocationBarcelona = Guid.Parse("A0000001-0000-0000-0000-000000000002");
    public static readonly Guid VehicleEconomy = Guid.Parse("B0000001-0000-0000-0000-000000000001");
    public static readonly Guid VehicleSuv = Guid.Parse("B0000001-0000-0000-0000-000000000002");
    public static readonly Guid ReservationSample = Guid.Parse("C0000001-0000-0000-0000-000000000001");
}
