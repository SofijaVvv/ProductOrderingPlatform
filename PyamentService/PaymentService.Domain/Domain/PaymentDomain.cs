using Microsoft.Extensions.Logging;
using PaymentService.Domain.Interface;
using PaymentService.Model.Dto;
using PaymentService.Model.Exceptions;
using PaymentService.Model.Extenetions;
using PaymentService.Model.Models;
using PaymentService.Repository.Interface;
using Stripe;
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
		var stripePaymentMethod = MapPaymentMethodTypeToStripe(paymentRequest.PaymentMethod);
		var testPaymentMethod = GetTestPaymentMethod(paymentRequest.PaymentMethod);
		var options = new PaymentIntentCreateOptions
		{
			Amount = (long)(paymentRequest.Amount * 100),
			Currency = "usd",
			PaymentMethodTypes = new List<string> { stripePaymentMethod }
		};

		PaymentIntent paymentIntent;

		try
		{
			var service = new PaymentIntentService();
			paymentIntent = await service.CreateAsync(options);

			paymentIntent = await service.ConfirmAsync(paymentIntent.Id, new PaymentIntentConfirmOptions
			{
				PaymentMethod = testPaymentMethod
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

		_eventPublisher.PublishAsync("payment.status",
			$"The payment status is: {payment.PaymentStatus}");

		return payment;
	}

	private string MapPaymentMethodTypeToStripe(PaymentMethod paymentMethod)
	{
		switch (paymentMethod)
		{
			case PaymentMethod.CreditCard:
				return "card";
			case PaymentMethod.BackTransfer:
				return "bank";
			default:
				throw new Exception($"Unsupported payment method: {paymentMethod}");
		}
	}

	private string GetTestPaymentMethod(PaymentMethod paymentMethod)
	{
		return paymentMethod switch
		{
			PaymentMethod.CreditCard => "pm_card_visa",
			PaymentMethod.BackTransfer => "pm_bank_transfer",
			_ => throw new Exception($"Unsupported test payment method: {paymentMethod}")
		};
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
