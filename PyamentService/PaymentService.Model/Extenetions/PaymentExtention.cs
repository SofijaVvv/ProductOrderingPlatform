using PaymentService.Model.Dto;
using PaymentService.Model.Models;

namespace PaymentService.Model.Extenetions;

public static class PaymentExtention
{
	public static Payment ToPayment(this PaymentRequest paymentRequest)
	{
		return new Payment
		{
			Amount = paymentRequest.Amount,
			PaymentStatus = paymentRequest.PaymentStatus,
			PaymentMethod = paymentRequest.PaymentMethod
		};
	}

	public static PaymentResponse ToResponse(this Payment response)
	{
		return new PaymentResponse
		{
			Id = response.Id,
			Amount = response.Amount,
			PaymentStatus = response.PaymentStatus,
			PaymentMethod = response.PaymentMethod,
			CreatedAt = response.CreatedAt
		};
	}

	public static List<PaymentResponse> ToResponse(this List<Payment> response)
	{
		return response.Select(payment => payment.ToResponse()).ToList();
	}
}
