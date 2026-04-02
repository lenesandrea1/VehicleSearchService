namespace VehicleSearchService.Application.Features.Reservations;

public interface ICreateReservationCommandHandler
{
    Task<CreateReservationResult> HandleAsync(CreateReservationCommand command, CancellationToken cancellationToken = default);
}
