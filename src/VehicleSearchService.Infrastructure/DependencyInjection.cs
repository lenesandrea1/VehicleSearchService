using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using VehicleSearchService.Application.Abstractions.Catalog;
using VehicleSearchService.Application.Abstractions.Messaging;
using VehicleSearchService.Application.Abstractions.Persistence;
using VehicleSearchService.Infrastructure.Catalog;
using VehicleSearchService.Infrastructure.Messaging;
using VehicleSearchService.Infrastructure.Persistence;

namespace VehicleSearchService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

        services.AddDbContext<VssDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        services.AddScoped<ILocationReadRepository, LocationReadRepository>();
        services.AddScoped<IVehicleReadRepository, VehicleReadRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();

        services.AddScoped<IDomainEventPublisher, InMemoryDomainEventPublisher>();
        services.AddScoped<IVehicleReservedEventHandler, LoggingVehicleReservedEventHandler>();

        AddCatalog(services, configuration);

        return services;
    }

    private static void AddCatalog(IServiceCollection services, IConfiguration configuration)
    {
        if (!configuration.GetValue("Catalog:Enabled", true))
        {
            services.AddScoped<ICatalogReader, NoOpCatalogReader>();
            return;
        }

        var connectionString = configuration["Catalog:ConnectionString"]
            ?? throw new InvalidOperationException(
                "Catalog:ConnectionString is required when Catalog:Enabled is true.");

        var databaseName = configuration["Catalog:DatabaseName"] ?? "vehiclesearch_catalog";

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddScoped<ICatalogReader>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return new MongoCatalogReader(client.GetDatabase(databaseName));
        });
    }
}
