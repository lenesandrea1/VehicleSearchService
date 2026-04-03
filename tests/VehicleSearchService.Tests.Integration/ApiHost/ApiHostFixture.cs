using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace VehicleSearchService.Tests.Integration.ApiHost;

public sealed class ApiHostFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("RunMigrations", "false");
        builder.UseSetting("Catalog:Enabled", "false");
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Server=127.0.0.1;Port=3306;Database=vehiclesearch_test;User=root;Password=unused;");
    }
}
