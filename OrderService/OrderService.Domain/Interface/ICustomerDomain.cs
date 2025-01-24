using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Domain.Interface;

public interface ICustomerDomain
{
	Task<List<Customer>> GetAllAsync();
	Task<Customer> GetByIdAsync(int id);
	Task<Customer> AddAsync(Customer customer);
	Task Update(int customerId, UpdateCustomerRequest updateCustomerRequest);
	Task<bool> DeleteAsync(int id);
}
