using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Model.Extentions;

public static class OrderPaymentExtentions
{




	public static OrderPaymentResponse ToResponse(this OrderResponse response, PaymentDto payment)
	{
		return new OrderPaymentResponse
		{
			Id = response.Id,
			CustomerId = response.CustomerId,
			OrderStatus = response.OrderStatus,
			Payment = new PaymentDto
			{
				Amount = payment.Amount,
				PaymentStatus = payment.PaymentStatus,
				OrderId = payment.OrderId,
				PaymentMethod = payment.PaymentMethod
			},
			CreatedAt = response.CreatedAt

		};
	}
}
