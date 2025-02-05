using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Infrastructure.Stripe.Interface;
using PaymentService.Model.Dto;
using Stripe;

namespace PaymentService.Infrastructure.Stripe.Stripe;

public class PaymentProcessing : IPaymentProcessing
{
	private readonly string _defaultCurrency;
	private readonly ILogger<PaymentProcessing> _logger;

	public PaymentProcessing(ILogger<PaymentProcessing> logger, IConfiguration configuration)
	{
		_defaultCurrency = configuration["PaymentConfig:DefaultCurrency"]
		                   ?? throw new InvalidOperationException("PaymentConfig:DefaultCurrency is missing");
		_logger = logger;
	}

	public async Task<PaymentIntent> ProcessPaymentAsync(PaymentRequest paymentRequest)
	{
		var paymentMethod = paymentRequest.PaymentMethod;

		var options = new PaymentIntentCreateOptions
		{
			Amount = (long)(paymentRequest.Amount * 100),
			Currency = _defaultCurrency,
			PaymentMethodTypes = new List<string> { "card" }
		};

		PaymentIntent paymentIntent;
		try
		{
			var service = new PaymentIntentService();
			paymentIntent = await service.CreateAsync(options);

			paymentIntent = await service.ConfirmAsync(paymentIntent.Id, new PaymentIntentConfirmOptions
			{
				PaymentMethod = paymentMethod
			});

			if (paymentIntent.Status != "succeeded")
			{
				throw new Exception("Payment confirmation failed");
			}
		}
		catch (StripeException ex)
		{
			_logger.LogError("Stripe error: {ErrorMessage}", ex.Message);
			_logger.LogError("Stripe error details: {ErrorDetails}", ex.StackTrace);
			throw new Exception($"Payment creation failed: {ex.Message}", ex);
		}

		return paymentIntent;
	}
}
