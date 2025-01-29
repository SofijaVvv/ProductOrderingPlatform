using OrderService.Model.Enum;

namespace OrderService.Model.Dto;

public class OrderPaymentRequest
{
	public PaymentMethod PaymentMethod { get; set; }
	public int OrderId { get; set; }
}
