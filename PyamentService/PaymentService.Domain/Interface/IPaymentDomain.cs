using PaymentService.Model.Dto;
using PaymentService.Model.Models;
using Stripe;

namespace PaymentService.Domain.Interface;

public interface IPaymentDomain
{
	Task<List<Payment>> GetAllAsync();
	Task<Payment> GetByIdAsync(int id);
	Task AddAsync(PaymentRequest paymentRequest);
	Task<bool> DeleteAsync(int id);
	Task<Refund> RefundAsync(string paymentIntentId, decimal amount );
	Task UpdateAsync(int id, UpdatePaymentRequest updatePaymentRequest);
}
