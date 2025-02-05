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

	public async Task SaveAsync()
	{
		await _context.SaveChangesAsync();
	}
}
