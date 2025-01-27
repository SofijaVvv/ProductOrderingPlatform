namespace OrderService.Model.Dto;

public class OrderItemRequest
{
	public int OrderId { get; set; }
	public string ProductId { get; set; }
	public int Quantity { get; set; }
}
