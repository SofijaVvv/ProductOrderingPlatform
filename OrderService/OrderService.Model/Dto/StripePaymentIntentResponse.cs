namespace OrderService.Model.Dto;

public class StripePaymentIntentResponse
{
	public string Id { get; set; }
	public string Object { get; set; }
	public int Amount { get; set; }
	public int AmountReceived { get; set; }
	public string ClientSecret { get; set; }
	public string Status { get; set; }
	public string LatestCharge { get; set; }
	public string PaymentMethod { get; set; }
}
