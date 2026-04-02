using VehicleSearchService.Domain.Entities;

namespace VehicleSearchService.Application.Abstractions.Persistence;

public interface IVehicleReadRepository
{
    /// <summary>Vehículos en la localidad que la infraestructura considere candidatos (p. ej. estado disponible en persistencia).</summary>
    Task<IReadOnlyList<Vehicle>> ListCandidatesAtLocationAsync(Guid locationId, CancellationToken cancellationToken = default);
}
