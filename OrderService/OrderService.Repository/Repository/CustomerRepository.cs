using OrderService.Model.Models;
using OrderService.Repository.Context;
using OrderService.Repository.Interface;

namespace OrderService.Repository.Repository;

public class CustomerRepository : GenericRepository<Customer>,ICustomerRepository
{
	public CustomerRepository(ApplicationDbContext context) : base(context)
	{
	}
}
