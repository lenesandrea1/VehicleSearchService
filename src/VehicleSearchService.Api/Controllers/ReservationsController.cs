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
            return NotFound();
        }
        catch (PickupLocationNotFoundException)
        {
            return NotFound();
        }
        catch (ReturnLocationNotFoundException)
        {
            return NotFound();
        }
        catch (VehicleNotAvailableException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidReservationPeriodException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
