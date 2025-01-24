using System.ComponentModel.DataAnnotations;

namespace OrderService.Model.Models;

public class Customer
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	public string Phone { get; set; }

	[Required]
	public string Email { get; set; }

	public string Address { get; set; }

	[Required]
	public DateTime CreatedAt { get; set; }
}
