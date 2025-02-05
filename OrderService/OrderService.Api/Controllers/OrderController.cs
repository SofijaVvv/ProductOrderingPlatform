using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;

namespace OrderService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
	private readonly IOrderDomain _orderDomain;

	public OrderController(IOrderDomain orderDomain)
	{
		_orderDomain = orderDomain;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OrderResponse>))]
	public async Task<ActionResult> GetAllOrders()
	{
		var orders = await _orderDomain.GetAllAsync();

		return Ok(orders);
	}

	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponse))]
	public async Task<ActionResult> GetOrderById(int id)
	{
		var order = await _orderDomain.GetByIdAsync(id);
		return Ok(order);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderResponse))]
	public async Task<IActionResult> AddOrder([FromBody] OrderRequest orderRequest)
	{
		var order = await _orderDomain.AddAsync(orderRequest);

		return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
	}

	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> UpdateOrder(
		[FromRoute] int id,
		[FromBody] UpdateOrderRequest updateOrderRequest)
	{
		await _orderDomain.Update(id, updateOrderRequest);

		return NoContent();
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> DeleteOrder(int id)
	{
		await _orderDomain.DeleteAsync(id);
		return NoContent();
	}
}