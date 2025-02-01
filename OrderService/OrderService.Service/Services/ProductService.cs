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
			var response = await _httpClient.GetAsync($"api/Product/{productId}");

			if (response.IsSuccessStatusCode)
			{
				var product = await response.Content.ReadFromJsonAsync<ProductDto>();
				_logger.LogInformation("Product retrieved successfully: {Product}", product);
				return product;
			}
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Error fetching product {ProductId}", productId);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Unexpected error processing payment");
		}
		return null;
	}

	public async Task<List<ProductDto>> GetProductsByIdsAsync(List<string> productIds)
	{
		try
		{
			var idsQuery = string.Join("&ids=", productIds);
			var response = await _httpClient.GetAsync($"api/Product/Batch?ids={idsQuery}");

			if (response.IsSuccessStatusCode)
			{
				var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
				_logger.LogInformation("Products retrieved successfully: {Products}", products);
				return products ?? new List<ProductDto>();
			}
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Error fetching products {ProductIds}", productIds);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Unexpected error processing payment");
		}
		return new List<ProductDto>();
	}
}
