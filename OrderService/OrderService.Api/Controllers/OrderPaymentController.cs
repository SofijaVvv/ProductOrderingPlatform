using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderPaymentController : ControllerBase
{
	private readonly IOrderPaymentDomain _orderPaymentDomain;

	public OrderPaymentController(IOrderPaymentDomain orderPaymentDomain)
	{
		_orderPaymentDomain = orderPaymentDomain;
	}

	[HttpPost("PayOrder")]
	public async Task<IActionResult> PayOrderAsync([FromBody] OrderPaymentRequest request)
	{
			var response = await _orderPaymentDomain.PayOrderAsync(request);
			return Ok(response);
	}

}
