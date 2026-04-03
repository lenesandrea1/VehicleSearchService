using Microsoft.EntityFrameworkCore;
using VehicleSearchService.Application.Abstractions.Persistence;
using VehicleSearchService.Domain.Entities;
using VehicleSearchService.Domain.Enums;
using VehicleSearchService.Infrastructure.Persistence.Mapping;

namespace VehicleSearchService.Infrastructure.Persistence;

public sealed class VehicleReadRepository(VssDbContext db) : IVehicleReadRepository
{
    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await db.Vehicles
            .AsNoTracking()
            .Include(v => v.EnabledMarkets)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken)
            .ConfigureAwait(false);

        return row is null ? null : VehicleMapper.ToDomain(row);
    }

    public async Task<IReadOnlyList<Vehicle>> ListCandidatesAtLocationAsync(
        Guid locationId,
        CancellationToken cancellationToken = default)
    {
        var rows = await db.Vehicles
            .AsNoTracking()
            .Include(v => v.EnabledMarkets)
            .Where(v => v.LocationId == locationId && v.Status == VehicleStatus.Available)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rows.ConvertAll(VehicleMapper.ToDomain);
    }
}
