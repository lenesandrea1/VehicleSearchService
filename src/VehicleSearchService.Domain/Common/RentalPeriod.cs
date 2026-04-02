namespace VehicleSearchService.Domain.Common;

/// <summary>
/// Utilidades para periodos de alquiler en UTC.
/// Se modela el intervalo semiabierto <c>[inicio, fin)</c>: al instante <c>fin</c> el vehículo vuelve a estar libre.
/// </summary>
public static class RentalPeriod
{
    /// <summary>Indica si dos periodos [aStart, aEnd) y [bStart, bEnd) se intersectan.</summary>
    public static bool Overlaps(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd) =>
        aStart < bEnd && bStart < aEnd;
}
