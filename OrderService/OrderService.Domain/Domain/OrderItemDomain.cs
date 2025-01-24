using Microsoft.Extensions.Logging;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;
using OrderService.Model.Models;
using OrderService.Repository.Interface;

namespace OrderService.Domain.Domain;

public class OrderItemDomain : IOrderItemDomain
{

	private readonly IUnitOfWork _unitOfWork;
	private readonly IOrderItemRepository _orderItemRepository;
	private readonly ILogger<OrderItemDomain> _logger;

	public OrderItemDomain(IUnitOfWork unitOfWork,
		IOrderItemRepository orderItemRepository, ILogger<OrderItemDomain> logger)
	{
		_orderItemRepository = orderItemRepository;
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	public async Task<List<OrderItem>> GetAllAsync()
	{
		return await _orderItemRepository.GetAllAsync();
	}

	public async Task<OrderItem> GetByIdAsync(int id)
	{
		var orderItem = await _orderItemRepository.GetByIdAsync(id);
		if (orderItem == null) throw new Exception("orderItem not found");

		return orderItem;
	}

	public async Task<OrderItem> AddAsync(OrderItem orderItem)
	{
		_orderItemRepository.AddAsync(orderItem);
		await _unitOfWork.SaveAsync();
		return orderItem;
	}

	public async Task Update(int orderItemId, UpdateOrderItemRequest updateOrderItemRequest)
	{
		var orderItem = await _orderItemRepository.GetByIdAsync(orderItemId);
		if (orderItem == null) throw new Exception("orderItem not found");

		orderItem.OrderId = updateOrderItemRequest.OrderId;
		orderItem.ProductId = updateOrderItemRequest.ProductId;
		orderItem.Quantity = updateOrderItemRequest.Quantity;
		orderItem.Price = updateOrderItemRequest.Price;

		_orderItemRepository.Update(orderItem);
		await _unitOfWork.SaveAsync();
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var orderItem = await _orderItemRepository.GetByIdAsync(id);
		if (orderItem == null) throw new Exception("orderItem not found");

		await _orderItemRepository.DeleteAsync(id);
		return true;
	}
}
