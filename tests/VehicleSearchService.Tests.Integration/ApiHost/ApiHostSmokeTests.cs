using Microsoft.AspNetCore.Mvc.Testing;

namespace VehicleSearchService.Tests.Integration.ApiHost;

public sealed class ApiHostSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiHostSmokeTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public void WebApplicationFactory_creates_HttpClient_without_throwing()
    {
        var client = _factory.CreateClient();
        Assert.NotNull(client);
    }
}
