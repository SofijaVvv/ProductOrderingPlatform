using OrderService.Model.Dto;

namespace OrderService.Service.Interface;

public interface IPaymentService
{
	Task<bool> ProcessPaymentAsync(PaymentDto paymentDto);
}
