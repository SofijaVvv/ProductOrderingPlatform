using ProductService.Model.Models;

namespace ProductService.Repository.Interfaces;

public interface IProductRepository
{
	Task<List<Product>> GetAllAsync();
	Task<Product?> GetByIdAsync(string id);
	void Add(Product product);
	Task<bool> DeleteAsync(string id);
	void Update(Product product);
}
