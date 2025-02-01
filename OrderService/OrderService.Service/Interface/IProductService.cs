using OrderService.Model.Dto;

namespace OrderService.Service.Interface;

public interface IProductService
{
	Task<ProductDto?> GetProductByIdAsync(string productId);
	Task<List<ProductDto>> GetProductsByIdsAsync(List<string> productIds);
}
