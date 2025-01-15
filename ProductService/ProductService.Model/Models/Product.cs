using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Model.Models;

public class Product
{
	[BsonId]
	public ObjectId Id { get; set; }

	[BsonElement("name")]
	public string Name { get; set; }

	public string ImageFileName { get; set; }

	[BsonElement("image")]
	public string Image { get; set; }

	[BsonElement("brand")]
	public string Brand { get; set; }

	[BsonElement("price")]
	public decimal Price { get; set; }

	[BsonElement("category")]
	public string Category { get; set; }
}
