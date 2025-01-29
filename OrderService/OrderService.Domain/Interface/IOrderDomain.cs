using OrderService.Model.Dto;
using OrderService.Model.Models;

namespace OrderService.Domain.Interface;

public interface IOrderDomain
{
	Task<List<OrderResponse>> GetAllAsync();
	Task<OrderResponse> GetByIdAsync(int id);
	Task<OrderResponse> AddAsync(OrderRequest orderRequest);
	Task Update(int orderId, UpdateOrderRequest updateOrderRequest);
	Task<bool> DeleteAsync(int id);

}
