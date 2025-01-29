using System.ComponentModel.DataAnnotations;
using OrderService.Model.Enum;

namespace OrderService.Model.Models;

public class Order
{
	[Key]
	public int Id { get; set; }

	[Required]
	public int CustomerId { get; set; }

	[Required]
	public OrderStatus OrderStatus { get; set; }

	[Required]
	public DateTime CreatedAt { get; set; }

	public virtual Customer Customer { get; set; }
}
