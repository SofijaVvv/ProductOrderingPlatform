using OrderService.Model.Enum;

namespace OrderService.Model.Dto;

public class OrderPaymentResponse
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public OrderStatus OrderStatus { get; set; }
	public PaymentDto Payment { get; set; }
	public DateTime CreatedAt { get; set; }
}
