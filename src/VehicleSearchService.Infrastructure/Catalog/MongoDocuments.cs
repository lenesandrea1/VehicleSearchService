using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VehicleSearchService.Infrastructure.Catalog;

[BsonIgnoreExtraElements]
internal sealed class MarketDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";
}

[BsonIgnoreExtraElements]
internal sealed class VehicleTypeDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";
}
