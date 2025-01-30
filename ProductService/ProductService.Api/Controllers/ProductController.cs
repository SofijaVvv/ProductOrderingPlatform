using Microsoft.AspNetCore.Mvc;
using ProductService.Domain.Interfaces;
using ProductService.Model.Dto;
using ProductService.Model.Models;

namespace ProductService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
	private readonly IProductDomain _productDomain;

	public ProductController(IProductDomain productDomain)
	{
		_productDomain = productDomain;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductResponse>))]
	public async Task<ActionResult> GetAllProducts()
	{
		var products = await _productDomain.GetAllAsync();

		return Ok(products);
	}

	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
	public async Task<ActionResult> GetProductById(string id)
	{
		var product = await _productDomain.GetByIdAsync(id);

		return Ok(product);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductResponse))]
	public async Task<IActionResult> AddProduct([FromBody] ProductRequest productRequest)
	{
		var product = await _productDomain.AddAsync(productRequest);

		return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
	}

	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> UpdateProduct(
		[FromRoute] string id,
		[FromBody] UpdateProductRequest updateAccountRequest)
	{
		await _productDomain.UpdateAsync(id, updateAccountRequest);

		return NoContent();
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> DeleteProduct(string id)
	{
		await _productDomain.DeleteAsync(id);
		return NoContent();
	}

}
