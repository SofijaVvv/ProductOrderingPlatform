using ProductService.Model.Dto;
using ProductService.Model.Models;

namespace ProductService.Domain.Interfaces;

public interface IProductDomain
{
	Task<List<Product>> GetAllAsync();
	Task<Product> GetByIdAsync(string id);
	Task<Product> AddAsync(ProductRequest productRequest);
	Task<bool> DeleteAsync(string id);
	Task UpdateAsync(string id, UpdateProductRequest updateProductRequest);
}
