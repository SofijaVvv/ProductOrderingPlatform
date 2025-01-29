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
		var content = new StringContent(JsonSerializer.Serialize(paymentDto), Encoding.UTF8, "application/json");
		var response = await _httpClient.PostAsync("http://localhost:5278/api/Payment", content);

		if (response.IsSuccessStatusCode)
		{
			var responseContent = await response.Content.ReadAsStringAsync();
			_logger.LogInformation("Payment API response: {ResponseContent}", responseContent);

			var paymentResponse = JsonSerializer.Deserialize<PaymentDto>(responseContent);

			if (paymentResponse != null)
			{
					_logger.LogInformation("PaymentDto Id set to: {Id}", paymentDto.Id);
			}

			return true;
		}

		return false;
	}

}
