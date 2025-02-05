using PaymentService.Model.Dto;
using Stripe;

namespace PaymentService.Infrastructure.Stripe.Interface;

public interface IPaymentProcessing
{
	Task<PaymentIntent> ProcessPaymentAsync(PaymentRequest paymentRequest);
}
