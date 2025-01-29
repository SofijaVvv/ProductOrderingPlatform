using OrderService.Model.Enum;

namespace OrderService.Model.Dto;

public class OrderResponse
{
	public int Id { get; set; }

	public int CustomerId { get; set; }

	public OrderStatus OrderStatus { get; set; }

	public decimal Amount { get; set; }


	public DateTime CreatedAt { get; set; }


}
