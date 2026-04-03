using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleSearchService.Api.Contracts;
using VehicleSearchService.Application.Common.Exceptions;
using VehicleSearchService.Application.Features.VehicleSearch;

namespace VehicleSearchService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VehiclesController(ISearchVehiclesQueryHandler searchHandler) : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(SearchVehiclesResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SearchVehiclesResult>> Search(
        [FromQuery] SearchVehiclesRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var query = new SearchVehiclesQuery(
            request.PickupLocationId,
            request.ReturnLocationId,
            TimeNormalization.AsUtc(request.PickupAtUtc),
            TimeNormalization.AsUtc(request.ReturnAtUtc));

        try
        {
            var result = await searchHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
        catch (InvalidRentalTimeWindowException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid rental period.",
                detail: ex.Message);
        }
    }
}
