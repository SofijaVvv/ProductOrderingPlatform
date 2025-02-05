using Microsoft.Extensions.Logging;
using PaymentService.Domain.Interface;
using PaymentService.Infrastructure.Stripe.Interface;
using PaymentService.Model.Dto;
using PaymentService.Model.Enum;
using PaymentService.Model.Exceptions;
using PaymentService.Model.Extenetions;
using PaymentService.Model.Models;
using PaymentService.Repository.Interface;
using PymentService.Infrastructure.Interface;

namespace PaymentService.Domain.Domain;

public class PaymentDomain : IPaymentDomain
{
	private readonly IPaymentRepository _paymentRepository;
	private readonly IEventPublisher _eventPublisher;
	private readonly IPaymentProcessing _paymentProcessing;
	private readonly ILogger<PaymentDomain> _logger;

	public PaymentDomain(IPaymentRepository paymentRepository,
		IEventPublisher eventPublisher,
		ILogger<PaymentDomain> logger,
		IPaymentProcessing paymentProcessing)
	{
		_paymentRepository = paymentRepository;
		_eventPublisher = eventPublisher;
		_logger = logger;
		_paymentProcessing = paymentProcessing;
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

	public async Task AddAsync(PaymentRequest paymentRequest)
	{
		var payment = paymentRequest.ToPayment();
		payment.CreatedAt = DateTime.UtcNow;
		payment.PaymentStatus = PaymentStatus.Failed;

		try
		{
			await _paymentProcessing.ProcessPaymentAsync(paymentRequest);
			payment.PaymentStatus = PaymentStatus.Completed;

		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "Payment processing failed for Order {OrderId}", paymentRequest.OrderId);
		}

		_paymentRepository.Add(payment);
		await _paymentRepository.SaveAsync();

		var paymentEventMessage = new PaymentEventMessage
		{
			Id = payment.Id,
			Amount = payment.Amount,
			PaymentStatus = payment.PaymentStatus.ToString(),
			OrderId = payment.OrderId,
			PaymentMethod = payment.PaymentMethod,
			CreatedAt = payment.CreatedAt
		};

		_eventPublisher.PublishAsync("payment.status",
			paymentEventMessage);
	}
}
