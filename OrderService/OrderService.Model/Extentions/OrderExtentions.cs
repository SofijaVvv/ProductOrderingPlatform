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
			Amount = request.Amount
		};

	}

	public static OrderResponse ToResponse(this Order response)
	{
		return new OrderResponse
		{
			Id = response.Id,
			CustomerId = response.CustomerId,
			OrderStatus = response.OrderStatus,
			Amount = response.Amount,
			PaymentStatus = response.PaymentStatus,
			CreatedAt = response.CreatedAt
		};

	}

	public static List<OrderResponse> ToResponse(this List<Order> response)
	{
		return response.Select(order => order.ToResponse()).ToList();
	}
}
