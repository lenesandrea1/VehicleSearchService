namespace VehicleSearchService.Api;

internal static class TimeNormalization
{
    /// <summary>Interprets unspecified <see cref="DateTimeKind"/> as UTC for API input.</summary>
    public static DateTime AsUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
}
