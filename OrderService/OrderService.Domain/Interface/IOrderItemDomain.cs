using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Domain.Interface;

public interface IOrderItemDomain
{
	Task<List<OrderItem>> GetAllAsync();
	Task<OrderItem> GetByIdAsync(int id);
	Task<OrderItem> AddAsync(OrderItem orderItem);
	Task Update(int orderItemId, UpdateOrderItemRequest updateOrderItemRequest);
	Task<bool> DeleteAsync(int id);
}
