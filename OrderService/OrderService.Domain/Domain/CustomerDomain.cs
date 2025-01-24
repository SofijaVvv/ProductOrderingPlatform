using Microsoft.Extensions.Logging;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;
using OrderService.Model.Models;
using OrderService.Repository.Interface;

namespace OrderService.Domain.Domain;

public class CustomerDomain : ICustomerDomain
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICustomerRepository _customerRepository;
	private readonly ILogger<CustomerDomain> _logger;

	public CustomerDomain(ICustomerRepository customerRepository,
		ILogger<CustomerDomain> logger, IUnitOfWork unitOfWork)
	{
		_customerRepository = customerRepository;
		_logger = logger;
		_unitOfWork = unitOfWork;
	}


	public async Task<List<Customer>> GetAllAsync()
	{
		return await _customerRepository.GetAllAsync();
	}

	public async Task<Customer> GetByIdAsync(int id)
	{
		var customer = await _customerRepository.GetByIdAsync(id);
		if (customer == null) throw new Exception("Customer not found");

		return customer;
	}

	public async Task<Customer> AddAsync(Customer customer)
	{
		_customerRepository.AddAsync(customer);
		await _unitOfWork.SaveAsync();
		return customer;
	}

	public async Task Update(int customerId, UpdateCustomerRequest updateCustomerRequest)
	{
		var customer = await _customerRepository.GetByIdAsync(customerId);
		if (customer == null) throw new Exception("Customer not found");

		customer.Name = updateCustomerRequest.Name;
		customer.Email = updateCustomerRequest.Email;
		customer.Phone = updateCustomerRequest.Phone;
		customer.Address = updateCustomerRequest.Address;

		_customerRepository.Update(customer);
		await _unitOfWork.SaveAsync();
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var customer = await _customerRepository.GetByIdAsync(id);
		if (customer == null) throw new Exception("Customer not found");

		await _customerRepository.DeleteAsync(id);
		return true;
	}
}
