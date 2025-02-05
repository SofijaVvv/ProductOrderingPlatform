using PaymentService.Model.Models;

namespace PaymentService.Repository.Interface;

public interface IPaymentRepository
{
	Task<List<Payment>> GetAllAsync();
	Task<Payment?> GetByIdAsync(int id);
	void Add(Payment product);
	Task SaveAsync();

}
