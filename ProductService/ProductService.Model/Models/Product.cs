using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Model.Models;

public class Product
{
	[BsonElement("_id")]
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; set; }

	[BsonElement("name")]
	public string Name { get; set; }

	[BsonElement("image")]
	public string Image { get; set; }

	[BsonElement("brand")]
	public string Brand { get; set; }

	[BsonElement("price")]
	public decimal Price { get; set; }

	[BsonElement("category")]
	public string Category { get; set; }
}
