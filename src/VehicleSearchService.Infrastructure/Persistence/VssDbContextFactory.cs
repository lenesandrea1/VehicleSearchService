using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace VehicleSearchService.Infrastructure.Persistence;

/// <summary>Design-time factory for EF Core CLI migrations.</summary>
public sealed class VssDbContextFactory : IDesignTimeDbContextFactory<VssDbContext>
{
    public VssDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<VssDbContext>()
            .UseMySql(
                "Server=localhost;Port=3306;Database=vehiclesearch;User=root;Password=local;",
                new MySqlServerVersion(new Version(8, 0, 36)))
            .Options;

        return new VssDbContext(options);
    }
}
