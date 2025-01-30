using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Domain.Interface;
using PaymentService.Model.Dto;
using PaymentService.Model.Exceptions;
using PaymentService.Model.Extenetions;
using PaymentService.Model.Models;
using PaymentService.Repository.Interface;
using PymentService.Infrastructure.Interface;
using Stripe;

namespace PaymentService.Domain.Domain;

public class PaymentDomain : IPaymentDomain
{
	private readonly IPaymentRepository _paymentRepository;
	private readonly IEventPublisher _eventPublisher;
	private readonly string _defaultCurrency;
	private readonly ILogger<PaymentDomain> _logger;

	public PaymentDomain(IPaymentRepository paymentRepository,
		IEventPublisher eventPublisher,
		ILogger<PaymentDomain> logger, IConfiguration configuration)
	{
		_paymentRepository = paymentRepository;
		_defaultCurrency = configuration["PaymentConfig:DefaultCurrency"]
		                   ?? throw new InvalidOperationException("PaymentConfig is missing");
		_eventPublisher = eventPublisher;
		_logger = logger;
	}

	public async Task<List<Payment>> GetAllAsync()
	{
		return await _paymentRepository.GetAllAsync();
	}

	public async Task<Payment> GetByIdAsync(int  id)
	{
		var payment = await _paymentRepository.GetByIdAsync(id);
		if (payment == null) throw new NotFoundException("Payment not found");
		return payment;
	}

	public async Task<Payment> AddAsync(PaymentRequest paymentRequest)
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

		var payment = paymentRequest.ToPayment();
		payment.CreatedAt = DateTime.UtcNow;
		_paymentRepository.Add(payment);
		await _paymentRepository.SaveAsync();

		var paymentEventMessage = new PaymentEventMessage
		{
			Id = payment.Id,
			Amount = payment.Amount,
			PaymentStatus = payment.PaymentStatus,
			OrderId = payment.OrderId,
			PaymentMethod = payment.PaymentMethod,
			CreatedAt = payment.CreatedAt
		};

		_eventPublisher.PublishAsync("payment.status",
			paymentEventMessage);

		return payment;
	}



	public async Task<Refund> RefundAsync(string paymentIntentId, decimal amount )
	{
		var refundOptions = new RefundCreateOptions
		{
			PaymentIntent = paymentIntentId,
		};

		if (amount > 0)
		{
			refundOptions.Amount = (long)(amount * 100);
		}

		var refundService = new RefundService();

		try
		{
			var refund = await refundService.CreateAsync(refundOptions);
			_logger.LogInformation("Refund created successfully for PaymentIntent: {PaymentIntentId}", paymentIntentId);
			return refund;
		}
		catch (StripeException ex)
		{
			_logger.LogError("Stripe refund error: {ErrorMessage}", ex.Message);
			_logger.LogError("Stripe error details: {ErrorDetails}", ex.StackTrace);
			throw new Exception($"Refund creation failed: {ex.Message}", ex);
		}
	}



	public async Task<bool> DeleteAsync(int id)
	{
		var payment = await _paymentRepository.GetByIdAsync(id);
		if (payment == null) throw new NotFoundException("Payment not found");
		await _paymentRepository.DeleteAsync(id);
		return true;
	}

	public async Task UpdateAsync(int id, UpdatePaymentRequest updatePaymentRequest)
	{
		var payment = await _paymentRepository.GetByIdAsync(id);
		if (payment == null) throw new NotFoundException("Payment not found");

		payment.Amount = updatePaymentRequest.Amount;
		payment.PaymentStatus = updatePaymentRequest.PaymentStatus;
		payment.PaymentMethod = updatePaymentRequest.PaymentMethod;

		_paymentRepository.Update(payment);
		await _paymentRepository.SaveAsync();
	}

}
