using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Model.Extentions;

public static class OrderExtentions
{
	public static Order ToOrder(this OrderRequest request)
	{
		return new Order
		{
			CustomerId = request.CustomerId,
			OrderStatus = request.OrderStatus,
		};

	}

	public static OrderResponse ToResponse(this Order response, decimal totalAmount)
	{
		return new OrderResponse
		{
			Id = response.Id,
			CustomerId = response.CustomerId,
			OrderStatus = response.OrderStatus,
			Amount = totalAmount,
			CreatedAt = response.CreatedAt
		};

	}

	public static List<OrderResponse> ToResponse(this List<Order> response, Dictionary<int, decimal> totalAmounts)
	{
		return response.Select(order => order.ToResponse(totalAmounts[order.Id])).ToList();
	}
}
