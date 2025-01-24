using OrderService.Model.Models;

namespace OrderService.Repository.Interface;

public interface IOrderRepository : IGenericRepository<Order>
{
	Task<List<Order>> GetAllOrdersAsync();
	Task<Order?> GetOrderByIdAsync(int id);
}
