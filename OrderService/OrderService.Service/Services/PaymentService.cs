using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrderService.Model.Dto;
using OrderService.Service.Interface;

namespace OrderService.Service.Services;

public class PaymentService : IPaymentService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<PaymentService> _logger;

	public PaymentService(HttpClient httpClient, ILogger<PaymentService> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<bool> ProcessPaymentAsync(PaymentDto paymentDto)
	{
		try
		{
			var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			var content = new StringContent(JsonSerializer.Serialize(paymentDto, options), Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync("api/Payment", content);

			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				_logger.LogInformation("Payment processed successfully: {ResponseContent}", responseContent);
				return true;
			}
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Payment failed with status {StatusCode}", ex.StatusCode);
		}
		catch (Exception e)
		{
			_logger.LogError("Error processing payment: {Message}", e.Message);
		}

		return false;
	}
}
