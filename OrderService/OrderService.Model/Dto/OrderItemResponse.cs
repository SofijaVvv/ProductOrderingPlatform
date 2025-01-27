namespace OrderService.Model.Dto;

public class OrderItemResponse
{
	public int Id { get; set; }
	public int OrderId { get; set; }
	public string ProductId { get; set; }
	public int Quantity { get; set; }
	public decimal Price { get; set; }
	public DateTime CreatedAt { get; set; }
	public ProductDto Product { get; set; }
}
