using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Domain.Interface;

public interface ICustomerDomain
{
	Task<List<Customer>> GetAllAsync();
	Task<Customer> GetByIdAsync(int id);
	Task<CustomerResponse> AddAsync(CustomerRequest customerRequest);
	Task Update(int customerId, UpdateCustomerRequest updateCustomerRequest);
	Task<bool> DeleteAsync(int id);
}