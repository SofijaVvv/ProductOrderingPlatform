using Microsoft.Extensions.Logging;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;
using OrderService.Model.Enum;
using OrderService.Service.Interface;
using OrderService.Model.Extentions;

namespace OrderService.Domain.Domain;

public class OrderPaymentDomain : IOrderPaymentDomain
{
	private readonly IOrderDomain _orderDomain;
	private readonly IPaymentService _paymentService;
	private readonly ILogger<OrderPaymentDomain> _logger;

	public OrderPaymentDomain(IOrderDomain orderDomain,
		IPaymentService paymentService,
		ILogger<OrderPaymentDomain> logger)
	{
		_orderDomain = orderDomain;
		_paymentService = paymentService;
		_logger = logger;
	}

	public async Task<OrderPaymentResponse> PayOrderAsync(OrderPaymentRequest request)
	{
		var order = await _orderDomain.GetByIdAsync(request.OrderId);
		if (order == null)
		{
			throw new Exception("Order not found");
		}

		var paymentDto = new PaymentDto
		{
			Amount = order.Amount,
			PaymentMethod = request.PaymentMethod,
			OrderId = order.Id
		};

		_logger.LogInformation("PaymentDto before processing: {PaymentDto}", paymentDto);

		var processPayment = await _paymentService.ProcessPaymentAsync(paymentDto);

		if (!processPayment)
		{
			throw new Exception("Payment processing failed");
		}

		paymentDto.PaymentStatus = PaymentStatus.Completed;

		return order.ToResponse(paymentDto);
	}
}
