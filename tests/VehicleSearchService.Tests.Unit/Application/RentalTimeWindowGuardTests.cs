using VehicleSearchService.Application.Common.Exceptions;
using VehicleSearchService.Application.Common.Validation;

namespace VehicleSearchService.Tests.Unit.Application;

public sealed class RentalTimeWindowGuardTests
{
    private static readonly DateTime Now = new(2026, 6, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Allows_pickup_after_now_with_return_after_pickup()
    {
        var pickup = Now.AddHours(1);
        var drop = Now.AddDays(2);
        var ex = Record.Exception(() => RentalTimeWindowGuard.EnsureValidForRequest(pickup, drop, Now));
        Assert.Null(ex);
    }

    [Fact]
    public void Rejects_pickup_before_now()
    {
        var pickup = Now.AddMinutes(-30);
        var drop = Now.AddDays(1);
        Assert.Throws<InvalidRentalTimeWindowException>(() =>
            RentalTimeWindowGuard.EnsureValidForRequest(pickup, drop, Now));
    }

    [Fact]
    public void Rejects_return_not_after_pickup()
    {
        var pickup = Now.AddHours(1);
        var drop = pickup;
        Assert.Throws<InvalidRentalTimeWindowException>(() =>
            RentalTimeWindowGuard.EnsureValidForRequest(pickup, drop, Now));
    }
}
