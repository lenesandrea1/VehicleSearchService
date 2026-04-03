namespace VehicleSearchService.Domain.Common;

/// <summary>
/// UTC rental interval helpers.
/// Intervals are half-open <c>[start, end)</c>: the vehicle is free again at <c>end</c>.
/// </summary>
public static class RentalPeriod
{
    /// <summary>Returns whether <c>[aStart, aEnd)</c> and <c>[bStart, bEnd)</c> intersect.</summary>
    public static bool Overlaps(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd) =>
        aStart < bEnd && bStart < aEnd;
}
