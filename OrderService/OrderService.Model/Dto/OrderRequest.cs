using OrderService.Model.Enum;

namespace OrderService.Model.Dto;

public class OrderRequest
{
	public int CustomerId { get; set; }

	public OrderStatus OrderStatus { get; set; }

	public decimal Amount { get; set; }
}
