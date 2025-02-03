using PaymentService.Model.Dto;
using Stripe;

namespace PymentService.Infrastructure.Interface;

public interface IPaymentProcessing
{
	Task<PaymentIntent> ProcessPaymentAsync(PaymentRequest paymentRequest);
}
