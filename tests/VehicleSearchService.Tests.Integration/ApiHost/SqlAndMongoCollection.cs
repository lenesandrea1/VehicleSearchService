namespace VehicleSearchService.Tests.Integration.ApiHost;

/// <summary>Shares one MySQL + Mongo pair for all tests in this collection (slower startup, faster suite).</summary>
[CollectionDefinition(nameof(SqlAndMongoCollection))]
public sealed class SqlAndMongoCollection : ICollectionFixture<SqlAndMongoFixture>
{
}
