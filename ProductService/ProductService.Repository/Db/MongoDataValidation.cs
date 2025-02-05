using MongoDB.Bson;
using MongoDB.Driver;

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
}
