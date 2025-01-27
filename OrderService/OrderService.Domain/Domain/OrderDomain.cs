using Microsoft.Extensions.Logging;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;
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
	private readonly ILogger<OrderDomain> _logger;

	public OrderDomain(IUnitOfWork unitOfWork,
		IOrderRepository orderRepository, ILogger<OrderDomain> logger,
	IOrderItemRepository orderItemRepository,
		IProductService productService)
	{
		_unitOfWork = unitOfWork;
		_orderRepository = orderRepository;
		_logger = logger;
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

	public async Task<Order> GetByIdAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id);
		if (order == null) throw new Exception("Order not found");

		return order;
	}

	public async Task<Order> AddAsync(Order order)
	{
		_orderRepository.AddAsync(order);
		await _unitOfWork.SaveAsync();
		return order;
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
		return true;
	}

	public async Task<decimal> CalculateTotalAmount(int orderId)
	{
		var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId);
		var productIds = orderItems.Select(oi => oi.ProductId).Distinct().ToList();

		decimal totalAmount = 0;
		foreach (var productId in productIds)
		{
			var product = await _productService.GetProductByIdAsync(productId);
			if (product != null)
			{
				totalAmount += orderItems
					.Where(oi => oi.ProductId == productId)
					.Sum(oi => oi.Quantity * product.Price);
			}
		}

		return totalAmount;
	}

}
