using System.ComponentModel.DataAnnotations;
using PaymentService.Model.Enum;

namespace PaymentService.Model.Models;

public class Payment
{
	[Key]
	public int Id { get; set; }

	[Required]
	public decimal Amount { get; set; }
	[Required]
	public PaymentStatus PaymentStatus { get; set; }
	public int? OrderId { get; set; }
	[Required]
	public string PaymentMethod { get; set; }
	[Required]
	public DateTime CreatedAt { get; set; }
}
