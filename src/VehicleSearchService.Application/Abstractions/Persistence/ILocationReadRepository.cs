using VehicleSearchService.Domain.Entities;

namespace VehicleSearchService.Application.Abstractions.Persistence;

/// <summary>Read-only access to rental locations.</summary>
public interface ILocationReadRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
