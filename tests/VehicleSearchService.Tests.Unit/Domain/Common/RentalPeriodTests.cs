using FluentAssertions;
using VehicleSearchService.Domain.Common;

namespace VehicleSearchService.Tests.Unit.Domain.Common;

public sealed class RentalPeriodTests
{
    [Fact]
    public void Overlaps_returns_false_when_periods_are_adjacent_half_open()
    {
        var aStart = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var aEnd = new DateTime(2026, 6, 3, 10, 0, 0, DateTimeKind.Utc);
        var bStart = aEnd;
        var bEnd = new DateTime(2026, 6, 5, 10, 0, 0, DateTimeKind.Utc);

        RentalPeriod.Overlaps(aStart, aEnd, bStart, bEnd).Should().BeFalse();
    }

    [Fact]
    public void Overlaps_returns_true_when_periods_share_any_instant()
    {
        var aStart = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var aEnd = new DateTime(2026, 6, 5, 10, 0, 0, DateTimeKind.Utc);
        var bStart = new DateTime(2026, 6, 4, 10, 0, 0, DateTimeKind.Utc);
        var bEnd = new DateTime(2026, 6, 10, 10, 0, 0, DateTimeKind.Utc);

        RentalPeriod.Overlaps(aStart, aEnd, bStart, bEnd).Should().BeTrue();
    }
}
