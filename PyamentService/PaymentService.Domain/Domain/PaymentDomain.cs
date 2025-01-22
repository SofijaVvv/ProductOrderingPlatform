using Microsoft.Extensions.Logging;
using PaymentService.Domain.Interface;
using PaymentService.Model.Dto;
using PaymentService.Model.Exceptions;
using PaymentService.Model.Extenetions;
using PaymentService.Model.Models;
using PaymentService.Repository.Interface;
using Stripe;
using PaymentService.Model.Enum;
using PaymentMethod = PaymentService.Model.Enum.PaymentMethod;

namespace PaymentService.Domain.Domain;

public class PaymentDomain : IPaymentDomain
{
	private readonly IPaymentRepository _paymentRepository;
	private readonly IEventPublisher _eventPublisher;
	private readonly ILogger<PaymentDomain> _logger;

	public PaymentDomain(IPaymentRepository paymentRepository,IEventPublisher eventPublisher,ILogger<PaymentDomain> logger)
	{
		_paymentRepository = paymentRepository;
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
		var stripePaymentMethod = MapPaymentMethodToStripe(paymentRequest.PaymentMethod);
		var options = new PaymentIntentCreateOptions
		{
			Amount = (long)(paymentRequest.Amount * 100), // Stripe expects the amount in cents
			Currency = "usd",  // Set your currency, e.g., USD
			PaymentMethodTypes = new List<string> { stripePaymentMethod },  // Assuming card payments
			Metadata = new Dictionary<string, string>
			{
				{ "orderId", paymentRequest.OrderId.ToString() }
			},
		};

		var service = new PaymentIntentService();
		try
		{
			await service.CreateAsync(options);
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

		_eventPublisher.PublishAsync("payment.status",
			$"The payment status is: {payment.PaymentStatus}");

		return payment;
	}

	private string MapPaymentMethodToStripe(PaymentMethod paymentMethod)
	{
		switch (paymentMethod)
		{
			case PaymentMethod.CreditCard:
				return "card";
			case PaymentMethod.BackTransfer:
				return "bank_transfer";
			default:
				throw new Exception($"Unsupported payment method: {paymentMethod}");
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
