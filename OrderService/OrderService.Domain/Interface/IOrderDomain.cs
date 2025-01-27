using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Domain.Interface;

public interface IOrderDomain
{
	Task<List<Order>> GetAllAsync();
	Task<Order> GetByIdAsync(int id);
	Task<Order> AddAsync(Order order);
	Task Update(int orderId, UpdateOrderRequest updateOrderRequest);
	Task<bool> DeleteAsync(int id);

	Task UpdateOrderAmount(int orderId);
}
