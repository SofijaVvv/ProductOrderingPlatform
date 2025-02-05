using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PaymentService.Model.Enum;
using PaymentService.Model.Models;

namespace PaymentService.Repository.Context;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}
	public DbSet<Payment> Payments { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Payment>(entity =>
		{
			entity.Property(p => p.Amount)
				.HasColumnType("decimal(18, 2)");

			entity.Property(p => p.PaymentStatus)
				.HasConversion(new EnumToStringConverter<PaymentStatus>());
		});

	}
}
