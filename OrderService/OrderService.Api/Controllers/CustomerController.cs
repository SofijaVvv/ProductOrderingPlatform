using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;

namespace OrderService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
	private readonly ICustomerDomain _customerDomain;

	public CustomerController(ICustomerDomain customerDomain)
	{
		_customerDomain = customerDomain;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CustomerResponse>))]
	public async Task<ActionResult> GetAllCustomers()
	{
		var customers = await _customerDomain.GetAllAsync();

		return Ok(customers);
	}

	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerResponse))]
	public async Task<ActionResult> GetCustomerById(int id)
	{
		var customer = await _customerDomain.GetByIdAsync(id);
		return Ok(customer);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CustomerResponse))]
	public async Task<IActionResult> AddCustomer([FromBody] CustomerRequest customerRequest)
	{
		var customer = await _customerDomain.AddAsync(customerRequest);

		return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
	}

	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> UpdateCustomer(
		[FromRoute] int id,
		[FromBody] UpdateCustomerRequest updateCustomerRequest)
	{
		await _customerDomain.Update(id, updateCustomerRequest);

		return NoContent();
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> DeleteCustomer(int id)
	{
		await _customerDomain.DeleteAsync(id);
		return NoContent();
	}
}