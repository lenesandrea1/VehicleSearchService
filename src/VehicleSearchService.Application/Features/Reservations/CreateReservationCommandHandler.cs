using VehicleSearchService.Application.Abstractions.Messaging;
using VehicleSearchService.Application.Abstractions.Persistence;
using VehicleSearchService.Application.Common.Exceptions;
using VehicleSearchService.Application.Common.Validation;
using VehicleSearchService.Domain.Entities;
using VehicleSearchService.Domain.Events;
using VehicleSearchService.Domain.Exceptions;

namespace VehicleSearchService.Application.Features.Reservations;

public sealed class CreateReservationCommandHandler(
    IVehicleReadRepository vehicles,
    ILocationReadRepository locations,
    IReservationRepository reservations,
    IDomainEventPublisher domainEvents,
    TimeProvider time) : ICreateReservationCommandHandler
{
    public async Task<CreateReservationResult> HandleAsync(CreateReservationCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var utcNow = time.GetUtcNow().UtcDateTime;
        RentalTimeWindowGuard.EnsureValidForRequest(command.PickupAtUtc, command.ReturnAtUtc, utcNow);

        var vehicle = await vehicles
            .GetByIdAsync(command.VehicleId, cancellationToken)
            .ConfigureAwait(false);
        if (vehicle is null)
            throw new VehicleNotFoundException(command.VehicleId);

        var pickupLocation = await locations
            .GetByIdAsync(command.PickupLocationId, cancellationToken)
            .ConfigureAwait(false);
        if (pickupLocation is null)
            throw new PickupLocationNotFoundException(command.PickupLocationId);

        var returnLocation = await locations
            .GetByIdAsync(command.ReturnLocationId, cancellationToken)
            .ConfigureAwait(false);
        if (returnLocation is null)
            throw new ReturnLocationNotFoundException(command.ReturnLocationId);

        var blocking = await reservations
            .ListPotentiallyBlockingAsync(
                [vehicle.Id],
                command.PickupAtUtc,
                command.ReturnAtUtc,
                cancellationToken)
            .ConfigureAwait(false);

        if (!vehicle.CanBeOfferedInSearch(pickupLocation, command.PickupAtUtc, command.ReturnAtUtc, blocking))
        {
            throw new VehicleNotAvailableException(
                "The vehicle is not available for the requested pickup location, market, dates, or an overlapping reservation exists.");
        }

        Reservation reservation;
        try
        {
            reservation = Reservation.Create(
                command.VehicleId,
                command.PickupLocationId,
                command.ReturnLocationId,
                command.PickupAtUtc,
                command.ReturnAtUtc);
        }
        catch (DomainException ex)
        {
            throw new InvalidReservationPeriodException(ex.Message);
        }

        await reservations.AddAsync(reservation, cancellationToken).ConfigureAwait(false);

        var occurred = DateTime.UtcNow;
        await domainEvents
            .PublishAsync(
                [
                    new VehicleReservedEvent(
                        reservation.Id,
                        reservation.VehicleId,
                        reservation.PickupLocationId,
                        reservation.ReturnLocationId,
                        reservation.PickupAtUtc,
                        reservation.ReturnAtUtc,
                        occurred)
                ],
                cancellationToken)
            .ConfigureAwait(false);

        return new CreateReservationResult(reservation.Id);
    }
}
