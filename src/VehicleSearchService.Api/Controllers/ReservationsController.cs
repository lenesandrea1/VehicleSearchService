using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleSearchService.Api.Contracts;
using VehicleSearchService.Application.Common.Exceptions;
using VehicleSearchService.Application.Features.Reservations;

namespace VehicleSearchService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReservationsController(ICreateReservationCommandHandler createHandler) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateReservationResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateReservationResult>> Create(
        [FromBody] CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var command = new CreateReservationCommand(
            request.VehicleId,
            request.PickupLocationId,
            request.ReturnLocationId,
            TimeNormalization.AsUtc(request.PickupAtUtc),
            TimeNormalization.AsUtc(request.ReturnAtUtc));

        try
        {
            var result = await createHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            return Created($"/api/reservations/{result.ReservationId}", result);
        }
        catch (VehicleNotFoundException)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Vehicle not found.");
        }
        catch (PickupLocationNotFoundException)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Pickup location not found.");
        }
        catch (ReturnLocationNotFoundException)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Return location not found.");
        }
        catch (VehicleNotAvailableException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Vehicle not available.",
                detail: ex.Message);
        }
        catch (InvalidReservationPeriodException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid reservation period.",
                detail: ex.Message);
        }
    }
}
