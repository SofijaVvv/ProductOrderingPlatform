using MongoDB.Bson;
using MongoDB.Driver;
using ProductService.Model.Models;
using ProductService.Repository.Interfaces;

namespace ProductService.Repository;



public class ProductRepository : IProductRepository
{
	private readonly IMongoCollection<Product> _collection;

	public ProductRepository(IMongoDatabase database)
	{
		_collection = database.GetCollection<Product>("Products");
	}

	public async Task<List<Product>> GetAllAsync()
	{
		return await _collection.Find(_ => true).ToListAsync();
	}

	public async Task<Product?> GetByIdAsync(string id)
	{
		var objectId = new ObjectId(id);
		return await _collection.Find(p => p.Id == objectId).FirstOrDefaultAsync();
	}

	public async Task AddAsync(Product product)
	{
		await _collection.InsertOneAsync(product);
	}

	public async Task<bool> DeleteAsync(string id)
	{
		var objectId = new ObjectId(id);
		var result = await _collection.DeleteOneAsync(p => p.Id == objectId);
		return result.DeletedCount > 0;
	}


	public async Task UpdateAsync(Product product)
	{
		var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
		var update = Builders<Product>.Update
			.Set(p => p.Name, product.Name)
			.Set(p => p.Image, product.Image)
			.Set(p => p.Brand, product.Brand)
			.Set(p => p.Price, product.Price)
			.Set(p => p.Category, product.Category);

		 await _collection.UpdateOneAsync(filter, update);
	}


}
