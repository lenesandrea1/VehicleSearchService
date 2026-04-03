namespace VehicleSearchService.Tests.Integration.ApiHost;

[CollectionDefinition(nameof(SqlAndMongoCollection))]
public sealed class SqlAndMongoCollection : ICollectionFixture<SqlAndMongoFixture>
{
}
