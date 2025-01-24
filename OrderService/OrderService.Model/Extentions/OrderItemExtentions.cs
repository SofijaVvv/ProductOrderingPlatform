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
			Price = request.Price
		};

	}

	public static OrderItemResponse ToResponse(this OrderItem response)
	{
		return new OrderItemResponse
		{
			Id = response.Id,
			OrderId = response.OrderId,
			ProductId = response.ProductId,
			Quantity = response.Quantity,
			Price = response.Price,
			CreatedAt = response.CreatedAt
		};

	}

	public static List<OrderItemResponse> ToResponse(this List<OrderItem> response)
	{
		return response.Select(orderItem => orderItem.ToResponse()).ToList();
	}
}
