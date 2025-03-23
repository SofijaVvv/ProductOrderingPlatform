using Microsoft.EntityFrameworkCore;
using OrderService.Model.Models;
using OrderService.Repository.Context;
using OrderService.Repository.Interface;

namespace OrderService.Repository.Repository;

public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
{
	private readonly ApplicationDbContext _context;

	public OrderItemRepository(ApplicationDbContext context) : base(context)
	{
		_context = context;
	}

	public async Task<List<OrderItem>> GetOrderItemsAsync()
	{
		var orderItems = await _context.OrderItems
			.Include(o => o.Order)
			.ToListAsync();

		return orderItems;
	}

	public async Task<List<OrderItem>> GetByOrderIdAsync(int orderId)
	{
		return await _context.OrderItems
			.Where(o => o.OrderId == orderId)
			.ToListAsync();
	}

	public override async Task<List<OrderItem>> GetAllAsync()
	{
		return await GetOrderItemsAsync();
	}

	public override async Task<OrderItem?> GetByIdAsync(int id)
	{
		var orderItem = await _context.OrderItems
			.Include(o => o.Order)
			.FirstOrDefaultAsync(o => o.Id == id);

		return orderItem;
	}
}
