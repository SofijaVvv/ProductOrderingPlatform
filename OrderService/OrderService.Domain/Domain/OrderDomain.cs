using Microsoft.Extensions.Logging;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;
using OrderService.Model.Enum;
using OrderService.Model.Extentions;
using OrderService.Model.Models;
using OrderService.Repository.Interface;
using OrderService.Service.Interface;

namespace OrderService.Domain.Domain;

public class OrderDomain : IOrderDomain
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IOrderRepository _orderRepository;
	private readonly IOrderItemRepository _orderItemRepository;
	private readonly IProductService _productService;
	private readonly IPaymentService _paymentService;
	private readonly ILogger<OrderDomain> _logger;

	public OrderDomain(IUnitOfWork unitOfWork,
		IOrderRepository orderRepository, ILogger<OrderDomain> logger,
	IOrderItemRepository orderItemRepository,
		IProductService productService,
		IPaymentService paymentService)
	{
		_unitOfWork = unitOfWork;
		_orderRepository = orderRepository;
		_logger = logger;
		_paymentService = paymentService;
		_orderItemRepository = orderItemRepository;
		_productService = productService;
	}

	public async Task<List<OrderResponse>> GetAllAsync()
	{
		var orders = await _orderRepository.GetAllAsync();
		var totalAmounts = new Dictionary<int, decimal>();

		foreach (var order in orders)
		{
			var totalAmount = await CalculateTotalAmount(order.Id);
			totalAmounts[order.Id] = totalAmount;
		}

		return orders.ToResponse(totalAmounts);
	}

	public async Task<OrderResponse> GetByIdAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id);
		if (order == null) throw new Exception("Order not found");

		var totalAmount = await CalculateTotalAmount(order.Id);
		var orderResponse = order.ToResponse(totalAmount);

		return orderResponse;
	}

	public async Task<OrderPaymentResponse> PayOrderAsync(OrderPaymentRequest request)
	{
		var order = await _orderRepository.GetByIdAsync(request.OrderId);
		if (order == null)
		{
			throw new Exception("Order not found");
		}


		var totalAmount = await CalculateTotalAmount(order.Id);
		var orderResponse = order.ToResponse(totalAmount);

		var paymentDto = new PaymentDto
		{
			Amount = orderResponse.Amount,
			PaymentMethod = request.PaymentMethod,
			OrderId = orderResponse.Id
		};

		_logger.LogInformation("PaymentDto before processing: {PaymentDto}", paymentDto);

		var processPayment = await _paymentService.ProcessPaymentAsync(paymentDto);

		if (!processPayment)
		{
			throw new Exception("Payment processing failed");
		}

		orderResponse.OrderStatus = OrderStatus.Completed;
		paymentDto.PaymentStatus = PaymentStatus.Completed;

		var updateRequest = new UpdateOrderRequest
		{
			CustomerId = orderResponse.CustomerId,
			OrderStatus = orderResponse.OrderStatus
		};

		await Update(orderResponse.Id, updateRequest);

		return orderResponse.ToResponse(paymentDto);
	}

	public async Task<OrderResponse> AddAsync(OrderRequest orderRequest)
	{
		Order order = orderRequest.ToOrder();
		order.CreatedAt = DateTime.UtcNow;

		_orderRepository.AddAsync(order);
		await _unitOfWork.SaveAsync();

		var totalAmount = await CalculateTotalAmount(order.Id);
		var orderResponse = order.ToResponse(totalAmount);

		return orderResponse;
	}

	public async Task Update(int orderId, UpdateOrderRequest updateOrderRequest)
	{
		var order = await _orderRepository.GetByIdAsync(orderId);
		if (order == null) throw new Exception("Order not found");

		order.CustomerId = updateOrderRequest.CustomerId;
		order.OrderStatus = updateOrderRequest.OrderStatus;

		_orderRepository.Update(order);
		await _unitOfWork.SaveAsync();
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id);
		if (order == null) throw new Exception("Order not found");

		await _orderRepository.DeleteAsync(id);
		await _unitOfWork.SaveAsync();
		return true;
	}

	public async Task<decimal> CalculateTotalAmount(int orderId)
	{
		var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId);
		var productIds = orderItems.Select(oi => oi.ProductId).Distinct().ToList();

		var products = await _productService.GetProductsByIdsAsync(productIds);
		var productDict = products.ToDictionary(p => p.Id, p => p.Price);

		decimal totalAmount = 0;
		foreach (var orderItem in orderItems)
		{
			if (productDict.TryGetValue(orderItem.ProductId, out var price))
			{
				totalAmount += orderItem.Quantity * price;
			}
		}

		return totalAmount;
	}

}
