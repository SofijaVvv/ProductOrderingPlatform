using PaymentService.Model.Enum;

namespace PaymentService.Model.Dto;

public class PaymentResponse
{
	public int Id { get; set; }

	public decimal Amount { get; set; }

	public int? OrderId { get; set; }
	public PaymentStatus PaymentStatus { get; set; }
	public PaymentMethod PaymentMethod { get; set; }
	public DateTime CreatedAt { get; set; }
}
