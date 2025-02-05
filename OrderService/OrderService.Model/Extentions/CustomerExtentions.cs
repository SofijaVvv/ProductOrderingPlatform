using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Model.Extentions;

public static class CustomerExtentions
{
	public static Customer ToCustomer(this CustomerRequest request)
	{
		return new Customer
		{
			Name = request.Name,
			Phone = request.Phone,
			Email = request.Email,
			Address = request.Address
		};
	}

	public static CustomerResponse ToResponse(this Customer response)
	{
		return new CustomerResponse
		{
			Id = response.Id,
			Name = response.Name,
			Phone = response.Phone,
			Email = response.Email,
			Address = response.Address
		};
	}
}
