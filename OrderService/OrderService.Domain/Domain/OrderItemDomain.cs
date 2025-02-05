using Microsoft.Extensions.Logging;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;
using OrderService.Model.Exceptions;
using OrderService.Model.Extentions;
using OrderService.Repository.Interface;
using OrderService.Service.Interface;

namespace OrderService.Domain.Domain;

public class OrderItemDomain : IOrderItemDomain
{
	private readonly ILogger<OrderItemDomain> _logger;
	private readonly IOrderItemRepository _orderItemRepository;
	private readonly IProductService _productService;

	private readonly IUnitOfWork _unitOfWork;

	public OrderItemDomain(IUnitOfWork unitOfWork,
		IOrderItemRepository orderItemRepository,
		ILogger<OrderItemDomain> logger,
		IProductService productService)
	{
		_orderItemRepository = orderItemRepository;
		_unitOfWork = unitOfWork;
		_productService = productService;
		_logger = logger;
	}

	public async Task<List<OrderItemResponse>> GetAllAsync()
	{
		var orderItems = await _orderItemRepository.GetAllAsync();
		var productIds = orderItems.Select(oi => oi.ProductId).Distinct().ToList();

		var productMapping = new Dictionary<string, ProductDto>();
		foreach (var productId in productIds)
		{
			var product = await _productService.GetProductByIdAsync(productId);
			if (product != null) productMapping[productId] = product;
		}

		return orderItems.ToResponse(productMapping);
	}

	public async Task<OrderItemResponse> GetByIdAsync(int id)
	{
		var orderItem = await _orderItemRepository.GetByIdAsync(id);
		if (orderItem == null) throw new NotFoundException("orderItem not found");

		var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
		if (product == null) throw new NotFoundException("Product not found");


		var orderItemResponse = orderItem.ToResponse(new ProductDto
		{
			Id = product.Id,
			Name = product.Name,
			Price = product.Price,
			Category = product.Category
		});

		return orderItemResponse;
	}

	public async Task<OrderItemResponse> AddAsync(OrderItemRequest orderItemRequest)
	{
		var product = await _productService.GetProductByIdAsync(orderItemRequest.ProductId);
		if (product == null) throw new NotFoundException($"Product with ID {orderItemRequest.ProductId} not found.");

		var orderItem = orderItemRequest.ToOrderItem();
		orderItem.CreatedAt = DateTime.UtcNow;

		var orderItemResponse = orderItem.ToResponse(new ProductDto
		{
			Id = product.Id,
			Name = product.Name,
			Price = product.Price,
			Category = product.Category
		});

		_orderItemRepository.AddAsync(orderItem);

		await _unitOfWork.SaveAsync();

		return orderItemResponse;
	}

	public async Task Update(int orderItemId, UpdateOrderItemRequest updateOrderItemRequest)
	{
		var orderItem = await _orderItemRepository.GetByIdAsync(orderItemId);
		if (orderItem == null) throw new Exception("orderItem not found");

		var product = await _productService.GetProductByIdAsync(updateOrderItemRequest.ProductId);
		if (product == null)
		{
			_logger.LogError("Product with ID {ProductId} not found", updateOrderItemRequest.ProductId);
			throw new Exception("Product not found");
		}

		orderItem.OrderId = updateOrderItemRequest.OrderId;
		orderItem.ProductId = updateOrderItemRequest.ProductId;
		orderItem.Quantity = updateOrderItemRequest.Quantity;

		_orderItemRepository.Update(orderItem);
		await _unitOfWork.SaveAsync();
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var orderItem = await _orderItemRepository.GetByIdAsync(id);
		if (orderItem == null) throw new Exception("orderItem not found");

		await _orderItemRepository.DeleteAsync(id);
		await _unitOfWork.SaveAsync();

		return true;
	}
}
