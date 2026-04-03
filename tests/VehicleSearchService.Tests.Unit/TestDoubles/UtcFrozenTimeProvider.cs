namespace VehicleSearchService.Tests.Unit.TestDoubles;

internal sealed class UtcFrozenTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _utcNow;

    public UtcFrozenTimeProvider(DateTime utcInstant) =>
        _utcNow = new DateTimeOffset(DateTime.SpecifyKind(utcInstant, DateTimeKind.Utc));

    public override DateTimeOffset GetUtcNow() => _utcNow;
}
