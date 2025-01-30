using ProductService.Model.Dto;
using ProductService.Model.Models;

namespace ProductService.Domain.Interfaces;

public interface IProductDomain
{
	Task<List<ProductResponse>> GetAllAsync();
	Task<ProductResponse> GetByIdAsync(string id);
	Task<ProductResponse> AddAsync(ProductRequest productRequest);
	Task DeleteAsync(string id);
	Task UpdateAsync(string id, UpdateProductRequest updateProductRequest);
}
