using OrderService.Model.Models;

namespace OrderService.Repository.Interface;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
	Task<List<OrderItem>> GetByOrderIdAsync(int orderId);
}
