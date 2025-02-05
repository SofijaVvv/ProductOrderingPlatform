using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Interface;
using OrderService.Model.Dto;

namespace OrderService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderItemController : ControllerBase
{
	private readonly IOrderItemDomain _orderItemDomain;

	public OrderItemController(IOrderItemDomain orderItemDomain)
	{
		_orderItemDomain = orderItemDomain;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OrderItemResponse>))]
	public async Task<ActionResult> GetAllOrderItems()
	{
		var orderItems = await _orderItemDomain.GetAllAsync();

		return Ok(orderItems);
	}

	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderItemResponse))]
	public async Task<ActionResult> GetOrderItemById(int id)
	{
		var orderItem = await _orderItemDomain.GetByIdAsync(id);
		return Ok(orderItem);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderItemResponse))]
	public async Task<IActionResult> AddOrderItem([FromBody] OrderItemRequest orderItemRequest)
	{
		var product = await _orderItemDomain.AddAsync(orderItemRequest);

		return CreatedAtAction(nameof(GetOrderItemById), new { id = product.Id }, product);
	}

	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> UpdateOrderItem(
		[FromRoute] int id,
		[FromBody] UpdateOrderItemRequest updateOrderItemRequest)
	{
		await _orderItemDomain.Update(id, updateOrderItemRequest);

		return NoContent();
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> DeleteOrderItem(int id)
	{
		await _orderItemDomain.DeleteAsync(id);
		return NoContent();
	}
}