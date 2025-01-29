using OrderService.Model.Dto;

namespace OrderService.Domain.Interface;

public interface IOrderPaymentDomain
{
	Task<OrderPaymentResponse> PayOrderAsync(OrderPaymentRequest request);
}
