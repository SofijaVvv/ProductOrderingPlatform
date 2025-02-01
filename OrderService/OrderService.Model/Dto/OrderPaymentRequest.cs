
namespace OrderService.Model.Dto;

public class OrderPaymentRequest
{
	public string PaymentMethod { get; set; }
	public int OrderId { get; set; }
}
