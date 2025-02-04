using System.Text.Json.Serialization;
using OrderService.Model.Enum;

namespace OrderService.Model.Dto;

public class PaymentDto
{
	public decimal Amount { get; set; }
	public PaymentStatus PaymentStatus { get; set; }

	public int OrderId { get; set; }
	public string PaymentMethod { get; set; }
}
