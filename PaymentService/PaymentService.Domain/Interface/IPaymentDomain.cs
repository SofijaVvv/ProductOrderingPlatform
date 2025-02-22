using PaymentService.Model.Dto;
using PaymentService.Model.Models;
using Stripe;

namespace PaymentService.Domain.Interface;

public interface IPaymentDomain
{
	Task<List<Payment>> GetAllAsync();
	Task<Payment> GetByIdAsync(int id);
	Task AddAsync(PaymentRequest paymentRequest);
}
