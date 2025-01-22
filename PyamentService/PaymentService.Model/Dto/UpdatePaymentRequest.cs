using PaymentService.Model.Enum;

namespace PaymentService.Model.Dto;

public class UpdatePaymentRequest
{
	public decimal Amount { get; set; }
	public PaymentStatus PaymentStatus { get; set; }
	public PaymentMethod PaymentMethod { get; set; }
}
