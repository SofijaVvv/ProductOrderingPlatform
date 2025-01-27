using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Model.Extentions;

public static class OrderItemExtentions
{
	public static OrderItem ToOrderItem(this OrderItemRequest request)
	{
		return new OrderItem
		{
			OrderId = request.OrderId,
			ProductId = request.ProductId,
			Quantity = request.Quantity,
		};

	}

	public static OrderItemResponse ToResponse(this OrderItem orderItem, ProductDto product)
	{
		return new OrderItemResponse
		{
			Id = orderItem.Id,
			OrderId = orderItem.OrderId,
			ProductId = orderItem.ProductId,
			Quantity = orderItem.Quantity,
			Price = orderItem.Quantity * product.Price,
			CreatedAt = orderItem.CreatedAt,
			Product = new ProductDto
			{
				Id = product.Id,
				Name = product.Name,
				Price = product.Price
			}
		};

	}

	public static List<OrderItemResponse> ToResponse(this List<OrderItem> orderItems, Dictionary<string, ProductDto> productMapping)
	{
		return orderItems.Select(orderItem =>
		{
			if (productMapping.TryGetValue(orderItem.ProductId, out var product))
			{
				return orderItem.ToResponse(product);
			}
			throw new Exception($"Product with ID {orderItem.ProductId} not found in product mapping.");
		}).ToList();
	}


}
