using Microsoft.EntityFrameworkCore;
using VehicleSearchService.Infrastructure.Persistence.Entities;

namespace VehicleSearchService.Infrastructure.Persistence;

public sealed class VssDbContext(DbContextOptions<VssDbContext> options) : DbContext(options)
{
    public DbSet<LocationRecord> Locations => Set<LocationRecord>();
    public DbSet<VehicleRecord> Vehicles => Set<VehicleRecord>();
    public DbSet<VehicleMarketRecord> VehicleMarkets => Set<VehicleMarketRecord>();
    public DbSet<ReservationRecord> Reservations => Set<ReservationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VssDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
