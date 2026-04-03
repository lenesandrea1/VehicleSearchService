using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using VehicleSearchService.Application;
using VehicleSearchService.Infrastructure;
using VehicleSearchService.Infrastructure.Catalog;
using VehicleSearchService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

if (app.Configuration.GetValue("RunMigrations", true))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<VssDbContext>();
    await db.Database.MigrateAsync().ConfigureAwait(false);
    await PersistenceSeeding.SeedAsync(db).ConfigureAwait(false);
}

if (app.Configuration.GetValue("Catalog:Enabled", true)
    && app.Configuration.GetValue("Catalog:SeedOnStartup", true))
{
    var mongoClient = app.Services.GetService<IMongoClient>();
    var dbName = app.Configuration["Catalog:DatabaseName"] ?? "vehiclesearch_catalog";
    if (mongoClient is not null)
    {
        await MongoCatalogSeed
            .EnsureAsync(mongoClient.GetDatabase(dbName))
            .ConfigureAwait(false);
    }
}

await app.RunAsync().ConfigureAwait(false);

public partial class Program { }
