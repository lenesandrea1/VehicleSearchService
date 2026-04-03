using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;
using Testcontainers.MySql;

namespace VehicleSearchService.Tests.Integration.ApiHost;

public sealed class SqlAndMongoFixture : IAsyncLifetime
{
    private readonly MySqlContainer _mysql = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .WithDatabase("vehicle_it")
        .WithUsername("root")
        .WithPassword("it-test")
        .Build();

    private readonly MongoDbContainer _mongo = new MongoDbBuilder()
        .WithImage("mongo:7")
        .Build();

    public WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _mysql.StartAsync().ConfigureAwait(false);
        await _mongo.StartAsync().ConfigureAwait(false);

        var mySqlCs = _mysql.GetConnectionString();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.UseSetting("ConnectionStrings:DefaultConnection", mySqlCs);
            builder.UseSetting("RunMigrations", "true");
            builder.UseSetting("Catalog:Enabled", "true");
            builder.UseSetting("Catalog:SeedOnStartup", "true");
            builder.UseSetting("Catalog:ConnectionString", _mongo.GetConnectionString());
            builder.UseSetting("Catalog:DatabaseName", "vehiclesearch_catalog_it");

            builder.ConfigureTestServices(services =>
            {
                foreach (var d in services.ToList())
                {
                    if (d.ServiceType == typeof(TimeProvider))
                        services.Remove(d);
                }

                services.AddSingleton<TimeProvider>(
                    new UtcFrozenTimeProvider(new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)));
            });
        });
    }

    public async Task DisposeAsync()
    {
        if (Factory is not null)
            await Factory.DisposeAsync().ConfigureAwait(false);

        await _mysql.DisposeAsync().ConfigureAwait(false);
        await _mongo.DisposeAsync().ConfigureAwait(false);
    }
}
