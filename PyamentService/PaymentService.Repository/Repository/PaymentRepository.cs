using Microsoft.EntityFrameworkCore;
using PaymentService.Model.Models;
using PaymentService.Repository.Context;
using PaymentService.Repository.Interface;

namespace PaymentService.Repository.Repository;

public class PaymentRepository : IPaymentRepository
{
	private readonly ApplicationDbContext _context;

	public PaymentRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<List<Payment>> GetAllAsync()
	{
		return await _context.Payments.ToListAsync();
	}

	public async Task<Payment?> GetByIdAsync(int id)
	{
		return await _context.Payments.FindAsync(id);
	}

	public void Add(Payment product)
	{
		_context.Payments.Add(product);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var payment = await GetByIdAsync(id);

		if (payment == null) return false;

		_context.Payments.Remove(payment);
		return true;
	}

	public void Update(Payment product)
	{
		_context.Payments.Update(product);
	}

	public async Task SaveAsync()
	{
		await _context.SaveChangesAsync();
	}
}
