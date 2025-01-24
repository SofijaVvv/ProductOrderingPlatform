using Microsoft.EntityFrameworkCore;
using OrderService.Model.Models;
using OrderService.Repository.Context;
using OrderService.Repository.Interface;

namespace OrderService.Repository.Repository;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
	private readonly ApplicationDbContext _context;

	public OrderRepository(ApplicationDbContext context) : base(context)
	{
		_context = context;
	}

	public async Task<List<Order>> GetAllOrdersAsync()
	{
		var orders = await _context.Orders
			.Include(o => o.Customer)
			.ToListAsync();

		return orders;
	}

	public async Task<Order?> GetOrderByIdAsync(int id)
	{
		var order = await _context.Orders
			.Include(o => o.Customer)
			.FirstOrDefaultAsync(o => o.Id == id);

		return order;
	}

	public override async Task<List<Order>> GetAllAsync()
	{
		return await GetAllOrdersAsync();
	}

	public override async Task<Order?> GetByIdAsync(int id)
	{
		return await GetOrderByIdAsync(id);
	}

}
