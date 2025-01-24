using Microsoft.Extensions.Logging;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;
using OrderService.Model.Models;
using OrderService.Repository.Interface;

namespace OrderService.Domain.Domain;

public class OrderDomain : IOrderDomain
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IOrderRepository _orderRepository;
	private readonly ILogger<OrderDomain> _logger;

	public OrderDomain(IUnitOfWork unitOfWork,
		IOrderRepository orderRepository, ILogger<OrderDomain> logger)
	{
		_unitOfWork = unitOfWork;
		_orderRepository = orderRepository;
		_logger = logger;
	}

	public async Task<List<Order>> GetAllAsync()
	{
		return await _orderRepository.GetAllAsync();
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
		order.Amount = updateOrderRequest.Amount;

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
}
