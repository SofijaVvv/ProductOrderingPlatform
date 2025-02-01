using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Model.Dto;

public class ProductResponse
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Image { get; set; }
	public string ImageFileName { get; set; }
	public string Brand { get; set; }
	public decimal Price { get; set; }
	public string Category { get; set; }
}
