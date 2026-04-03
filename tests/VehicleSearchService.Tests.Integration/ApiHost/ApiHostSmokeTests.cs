namespace VehicleSearchService.Tests.Integration.ApiHost;

public sealed class ApiHostSmokeTests : IClassFixture<ApiHostFixture>
{
    private readonly ApiHostFixture _factory;

    public ApiHostSmokeTests(ApiHostFixture factory) => _factory = factory;

    [Fact]
    public void WebApplicationFactory_creates_HttpClient_without_throwing()
    {
        var client = _factory.CreateClient();
        Assert.NotNull(client);
    }
}
