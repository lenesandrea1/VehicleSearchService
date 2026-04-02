using FluentAssertions;
using VehicleSearchService.Domain.Entities;
using VehicleSearchService.Domain.Enums;
using VehicleSearchService.Domain.Exceptions;

namespace VehicleSearchService.Tests.Unit.Domain.Entities;

public sealed class ReservationTests
{
    [Fact]
    public void Create_throws_when_return_is_not_after_pickup()
    {
        var pickup = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc);
        var act = () => Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            pickup,
            pickup);

        act.Should().Throw<DomainException>()
            .WithMessage("*devolución*");
    }

    [Fact]
    public void Cancel_is_idempotent_when_already_cancelled()
    {
        var r = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 8, 5, 10, 0, 0, DateTimeKind.Utc));
        r.Cancel();
        r.Invoking(x => x.Cancel()).Should().NotThrow();
        r.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public void Cancel_throws_when_reservation_is_completed()
    {
        var r = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 9, 5, 10, 0, 0, DateTimeKind.Utc));
        r.Complete();

        r.Invoking(x => x.Cancel()).Should().Throw<DomainException>();
    }

    [Fact]
    public void Complete_throws_when_reservation_is_cancelled()
    {
        var r = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTime(2026, 10, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 10, 5, 10, 0, 0, DateTimeKind.Utc));
        r.Cancel();

        r.Invoking(x => x.Complete()).Should().Throw<DomainException>();
    }

    [Fact]
    public void ConflictsWithRentalRequest_is_false_for_other_vehicle()
    {
        var vehicleId = Guid.NewGuid();
        var otherVehicle = Guid.NewGuid();
        var r = Reservation.Create(
            otherVehicle,
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 11, 5, 10, 0, 0, DateTimeKind.Utc));

        r.ConflictsWithRentalRequest(
                vehicleId,
                new DateTime(2026, 11, 2, 10, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 11, 4, 10, 0, 0, DateTimeKind.Utc))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ConflictsWithRentalRequest_is_false_when_reservation_is_cancelled_even_if_dates_overlap()
    {
        var vehicleId = Guid.NewGuid();
        var r = Reservation.Create(
            vehicleId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTime(2026, 12, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 12, 5, 10, 0, 0, DateTimeKind.Utc));
        r.Cancel();

        r.ConflictsWithRentalRequest(
                vehicleId,
                new DateTime(2026, 12, 2, 10, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 12, 4, 10, 0, 0, DateTimeKind.Utc))
            .Should()
            .BeFalse();
    }
}
