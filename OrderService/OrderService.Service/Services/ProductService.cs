using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using OrderService.Model.Dto;
using OrderService.Service.Interface;

namespace OrderService.Service.Services;

public class ProductService : IProductService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<ProductService> _logger;

	public ProductService(HttpClient httpClient,ILogger<ProductService> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<ProductDto?> GetProductByIdAsync(string productId)
	{
		try
		{
			var response = await _httpClient.GetAsync($"http://localhost:5121/api/Product/{productId}");
			response.EnsureSuccessStatusCode();

			var jsonResponse = await response.Content.ReadAsStringAsync();
			_logger.LogInformation($"JSON Response: {jsonResponse}");

			var product = await response.Content.ReadFromJsonAsync<ProductDto>();
			_logger.LogInformation($"ID: {product?.Id}");
			return product;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error fetching product with ID {ProductId}", productId);
			return null;
		}

	}
}
