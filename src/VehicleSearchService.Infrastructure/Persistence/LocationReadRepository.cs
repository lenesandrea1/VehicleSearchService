using Microsoft.EntityFrameworkCore;
using VehicleSearchService.Application.Abstractions.Persistence;
using VehicleSearchService.Domain.Entities;

namespace VehicleSearchService.Infrastructure.Persistence;

public sealed class LocationReadRepository(VssDbContext db) : ILocationReadRepository
{
    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await db.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);

        if (row is null)
            return null;

        return new Location
        {
            Id = row.Id,
            MarketId = row.MarketId,
            Name = row.Name
        };
    }
}
