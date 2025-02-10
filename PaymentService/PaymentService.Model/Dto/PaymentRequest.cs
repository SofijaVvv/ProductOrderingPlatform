using PaymentService.Model.Enum;

namespace PaymentService.Model.Dto;

public class PaymentRequest
{
	public decimal Amount { get; set; }
	public PaymentStatus PaymentStatus { get; set; }
	public string PaymentMethod { get; set; }

	public int? OrderId { get; set; }

}
