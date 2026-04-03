using VehicleSearchService.Domain.Entities;

namespace VehicleSearchService.Application.Abstractions.Persistence;

public interface IVehicleReadRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Vehicles at the station that persistence treats as search candidates (e.g. available status).</summary>
    Task<IReadOnlyList<Vehicle>> ListCandidatesAtLocationAsync(Guid locationId, CancellationToken cancellationToken = default);
}
