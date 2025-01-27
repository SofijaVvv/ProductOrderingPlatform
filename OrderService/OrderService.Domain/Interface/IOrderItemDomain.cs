using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Domain.Interface;

public interface IOrderItemDomain
{
	Task<List<OrderItemResponse>> GetAllAsync();
	Task<OrderItemResponse> GetByIdAsync(int id);
	Task<OrderItemResponse> AddAsync(OrderItemRequest orderItemRequest);
	Task Update(int orderItemId, UpdateOrderItemRequest updateOrderItemRequest);
	Task<bool> DeleteAsync(int id);
}
