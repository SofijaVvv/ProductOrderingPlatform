using OrderService.Model.Models;

namespace OrderService.Repository.Interface;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
	Task<List<OrderItem>> GetOrderItemsAsync();
	Task<OrderItem?> GetOrderItemByIdAsync(int id);
	Task<List<OrderItem>> GetByOrderIdAsync(int orderId);
}
