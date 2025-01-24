using System.ComponentModel.DataAnnotations;

namespace OrderService.Model.Models;

public class OrderItem
{
	[Key]
	public int Id { get; set; }
	[Required]
	public int OrderId { get; set; }
	[Required]
	public string ProductId { get; set; }
	[Required]
	public int Quantity { get; set; }
	[Required]
	public decimal Price { get; set; }
	[Required]
	public DateTime CreatedAt { get; set; }

	public virtual Order Order { get; set; }
}
