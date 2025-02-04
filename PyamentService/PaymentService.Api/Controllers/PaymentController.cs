using Microsoft.AspNetCore.Mvc;
using PaymentService.Domain.Interface;
using PaymentService.Model.Dto;
using PaymentService.Model.Extenetions;
using PaymentService.Model.Models;

namespace PyamentService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
	private readonly IPaymentDomain _paymentDomain;

	public PaymentController(IPaymentDomain paymentDomain)
	{
		_paymentDomain = paymentDomain;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PaymentResponse>))]
	public async Task<ActionResult> GetAllPayments()
	{
		var payments = await _paymentDomain.GetAllAsync();

		var result = payments.ToResponse();
		return Ok(result);
	}

	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResponse))]
	public async Task<ActionResult<Payment>> GetPaymentById(int id)
	{
		var payment = await _paymentDomain.GetByIdAsync(id);
		var result = payment.ToResponse();
		return Ok(result);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest paymentRequest)
	{
		await _paymentDomain.AddAsync(paymentRequest);

		return NoContent();
	}

	[HttpPost("Refund")]
	public async Task<IActionResult> Refund(string paymentIntentId, decimal amount)
	{
			var refund = await _paymentDomain.RefundAsync(paymentIntentId, amount);
			return Ok(new { Message = "Refund successful", Refund = refund });
	}


	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> DeletePayment(int id)
	{
		await _paymentDomain.DeleteAsync(id);
		return NoContent();
	}

}
