using PaymentService.Model.Dto;
using PaymentService.Model.Models;

namespace PaymentService.Domain.Interface;

public interface IPaymentDomain
{
	Task<List<Payment>> GetAllAsync();
	Task<Payment> GetByIdAsync(int id);
	Task<Payment> AddAsync(PaymentRequest paymentRequest);
	Task<bool> DeleteAsync(int id);
	Task UpdateAsync(int id, UpdatePaymentRequest updatePaymentRequest);
}
