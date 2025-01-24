using OrderService.Model.Enum;

namespace OrderService.Model.Dto;

public class UpdateOrderRequest
{
	public int CustomerId { get; set; }

	public OrderStatus OrderStatus { get; set; }

	public decimal Amount { get; set; }
}
