using OrderService.Model.Dto;

namespace OrderService.Domain.Interface;

public interface IOrderDomain
{
	Task<List<OrderResponse>> GetAllAsync();
	Task<OrderResponse> GetByIdAsync(int id);
	Task<OrderResponse> AddAsync(OrderRequest orderRequest);
	Task<OrderPaymentResponse> PayOrderAsync(OrderPaymentRequest request);
	Task Update(int orderId, UpdateOrderRequest updateOrderRequest);
	Task<bool> DeleteAsync(int id);
}