using MongoDB.Bson;
using MongoDB.Driver;
using ProductService.Model.Models;

namespace ProductService.Repository.Db;

public class MongoDataValidation
{
	private readonly IMongoDatabase _database;

	public MongoDataValidation(IMongoDatabase database)
	{
		_database = database;
	}

	public void SetCollectionValidation()
	{
		var command = new BsonDocument
		{
			{ "collMod", "Products" },
			{ "validator", new BsonDocument
				{
					{ "$jsonSchema", new BsonDocument
						{
							{ "bsonType", "object" },
							{ "required", new BsonArray { "name", "brand", "image", "price", "category" } },
							{ "properties", new BsonDocument
								{
									{ "name", new BsonDocument { { "bsonType", "string" } } },
									{ "brand", new BsonDocument { { "bsonType", "string" } } },
									{ "image", new BsonDocument { { "bsonType", "string" } } },
									{ "price", new BsonDocument { { "bsonType", "decimal" } } },
									{ "category", new BsonDocument { { "bsonType", "string" } } }
								}
							}
						}
					}
				}
			}
		};

		_database.RunCommand<BsonDocument>(command);
	}

	public void TestInsertProduct()
	{
		try
		{
			var collection = _database.GetCollection<Product>("Products");
			var product = new Product
			{
				Name = "New Product",
				Brand = "Walmart",
				Image = "https://i.natgeofe.com/n/4f5aaece-3300-41a4-b2a8-ed2708a0a27c/domestic-dog_thumb_square.jpg?wp=1&w=136&h=136",
				Category = "Electronics"
			};

			collection.InsertOne(product);
			Console.WriteLine("Product inserted successfully.");
			Console.WriteLine("Connected to database: " + _database.DatabaseNamespace.DatabaseName);

		}
		catch (MongoWriteException ex)
		{
			Console.WriteLine("Error inserting document: " + ex.Message);
		}
	}
}
