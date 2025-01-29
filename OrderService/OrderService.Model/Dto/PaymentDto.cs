using System.Text.Json.Serialization;
using OrderService.Model.Enum;

namespace OrderService.Model.Dto;

public class PaymentDto
{
	[JsonPropertyName("id")]
	public int Id { get; set; }
	public decimal Amount { get; set; }
	public PaymentStatus PaymentStatus { get; set; }

	public int? OrderId { get; set; }
	public PaymentMethod PaymentMethod { get; set; }
}
