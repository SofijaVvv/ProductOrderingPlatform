using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderPaymentController : ControllerBase
{
	private readonly IOrderDomain _orderDomain;

	public OrderPaymentController(IOrderDomain orderDomain)
	{
		_orderDomain = orderDomain;
	}

	[HttpPost("PayOrder")]
	public async Task<IActionResult> PayOrderAsync([FromBody] OrderPaymentRequest request)
	{
		var response = await _orderDomain.PayOrderAsync(request);
		return Ok(response);
	}
}