using OrderService.Domain.Interface;
using OrderService.Model.Dto;
using OrderService.Model.Enum;
using OrderService.Model.Exceptions;
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

	public OrderDomain(
		IUnitOfWork unitOfWork,
		IOrderRepository orderRepository,
		IOrderItemRepository orderItemRepository,
		IProductService productService,
		IPaymentService paymentService)
	{
		_unitOfWork = unitOfWork;
		_orderRepository = orderRepository;
		_paymentService = paymentService;
		_orderItemRepository = orderItemRepository;
		_productService = productService;
	}

	public async Task<List<OrderResponse>> GetAllAsync()
	{
		var orders = await _orderRepository.GetAllAsync();

		var productIds = orders.SelectMany(o => o.OrderItem.Select(oi => oi.ProductId)).Distinct().ToList();

		var products = await _productService.GetProductsByIdsAsync(productIds);
		var productDict = products.ToDictionary(p => int.Parse(p.Id), p => p.Price);

		var totalAmounts = orders.ToDictionary(order => order.Id, order => CalculateTotalAmount(order, productDict));

		return orders.ToResponse(totalAmounts);
	}

	public async Task<OrderResponse> GetByIdAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id);
		if (order == null) throw new NotFoundException("Order not found");

		var productDict = await GetProductDictionaryByOrderIdAsync(order.Id);

		var totalAmount = CalculateTotalAmount(order, productDict);
		return order.ToResponse(totalAmount);
	}

	public async Task<OrderPaymentResponse> PayOrderAsync(OrderPaymentRequest request)
	{
		var order = await _orderRepository.GetByIdAsync(request.OrderId);
		if (order == null) throw new Exception("Order not found");

		var productDict = await GetProductDictionaryByOrderIdAsync(order.Id);

		var totalAmount = CalculateTotalAmount(order, productDict);
		var orderResponse = order.ToResponse(totalAmount);

		var paymentDto = new PaymentDto
		{
			Amount = orderResponse.Amount,
			PaymentMethod = request.PaymentMethod,
			OrderId = orderResponse.Id,
			PaymentStatus = PaymentStatus.Pending
		};

		var processPayment = await _paymentService.ProcessPaymentAsync(paymentDto);
		if (!processPayment) throw new Exception("Payment processing failed");

		return orderResponse.ToResponse(paymentDto);
	}

	public async Task<OrderResponse> AddAsync(OrderRequest orderRequest)
	{
		var order = orderRequest.ToOrder();
		order.CreatedAt = DateTime.UtcNow;

		_orderRepository.Add(order);
		await _unitOfWork.SaveAsync();

		var productDict = await GetProductDictionaryByOrderIdAsync(order.Id);

		var totalAmount = CalculateTotalAmount(order, productDict);
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

		_orderRepository.DeleteAsync(order);
		await _unitOfWork.SaveAsync();

		return true;
	}

	private async Task<Dictionary<int, decimal>> GetProductDictionaryByOrderIdAsync(int orderId)
	{
		var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId);
		var productIds = orderItems.Select(oi => oi.ProductId).Distinct().ToList();
		var products = await _productService.GetProductsByIdsAsync(productIds);
		return products.ToDictionary(p => int.Parse(p.Id), p => p.Price);
	}

	private decimal CalculateTotalAmount(Order order, Dictionary<int, decimal> productDict)
	{
		decimal totalAmount = 0;

		foreach (var orderItem in order.OrderItem)
			if (productDict.TryGetValue(int.Parse(orderItem.ProductId), out var price))
				totalAmount += orderItem.Quantity * price;

		return totalAmount;
	}
}
