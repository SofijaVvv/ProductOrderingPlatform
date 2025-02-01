using ProductService.Model.Models;

namespace ProductService.Repository.Interfaces;

public interface IProductRepository
{
	Task<List<Product>> GetAllAsync();
	Task<Product?> GetByIdAsync(string id);
	Task AddAsync(Product product);
	Task<bool> DeleteAsync(string id);
	Task<List<Product>> GetByIdsAsync(List<string> ids);
	Task UpdateAsync(Product product);
}
