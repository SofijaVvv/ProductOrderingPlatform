using OrderService.Domain.Interface;
using OrderService.Model.Dto;
using OrderService.Model.Exceptions;
using OrderService.Model.Extentions;
using OrderService.Model.Models;
using OrderService.Repository.Interface;

namespace OrderService.Domain.Domain;

public class CustomerDomain : ICustomerDomain
{
	private readonly ICustomerRepository _customerRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CustomerDomain(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
	{
		_customerRepository = customerRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<List<Customer>> GetAllAsync()
	{
		return await _customerRepository.GetAllAsync();
	}

	public async Task<Customer> GetByIdAsync(int id)
	{
		var customer = await _customerRepository.GetByIdAsync(id);
		if (customer == null) throw new NotFoundException("Customer not found");

		return customer;
	}

	public async Task<CustomerResponse> AddAsync(CustomerRequest customerRequest)
	{
		var customer = customerRequest.ToCustomer();

		_customerRepository.Add(customer);
		await _unitOfWork.SaveAsync();

		return customer.ToResponse();
	}

	public async Task Update(int customerId, UpdateCustomerRequest updateCustomerRequest)
	{
		var customer = await _customerRepository.GetByIdAsync(customerId);
		if (customer == null) throw new NotFoundException("Customer not found");

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
		if (customer == null) throw new NotFoundException("Customer not found");

		_customerRepository.DeleteAsync(customer);
		await _unitOfWork.SaveAsync();

		return true;
	}
}
