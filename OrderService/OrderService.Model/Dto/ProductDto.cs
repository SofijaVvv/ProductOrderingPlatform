namespace OrderService.Model.Dto;

public class ProductDto
{
	public IdObject Id { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public string Category { get; set; }
}
