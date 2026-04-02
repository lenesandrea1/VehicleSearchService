using VehicleSearchService.Domain.Entities;

namespace VehicleSearchService.Application.Abstractions.Persistence;

public interface ILocationReadRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
