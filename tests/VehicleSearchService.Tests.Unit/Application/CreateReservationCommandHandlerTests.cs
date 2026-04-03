using Moq;
using VehicleSearchService.Application.Abstractions.Messaging;
using VehicleSearchService.Application.Abstractions.Persistence;
using VehicleSearchService.Application.Common.Exceptions;
using VehicleSearchService.Application.Features.Reservations;
using VehicleSearchService.Domain.Entities;

namespace VehicleSearchService.Tests.Unit.Application;

public sealed class CreateReservationCommandHandlerTests
{
    [Fact]
    public async Task Throws_when_vehicle_missing()
    {
        var vehicleId = Guid.Parse("30000000-0000-0000-0000-000000000001");

        var vehicles = new Mock<IVehicleReadRepository>();
        vehicles
            .Setup(v => v.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        var locations = new Mock<ILocationReadRepository>();
        var reservations = new Mock<IReservationRepository>();
        var publisher = new Mock<IDomainEventPublisher>();

        var handler = new CreateReservationCommandHandler(
            vehicles.Object,
            locations.Object,
            reservations.Object,
            publisher.Object);

        var command = new CreateReservationCommand(
            vehicleId,
            Guid.Parse("40000000-0000-0000-0000-000000000001"),
            Guid.Parse("50000000-0000-0000-0000-000000000001"),
            new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 4, 3, 10, 0, 0, DateTimeKind.Utc));

        await Assert.ThrowsAsync<VehicleNotFoundException>(() => handler.HandleAsync(command));
    }
}
