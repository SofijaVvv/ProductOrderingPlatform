using Microsoft.EntityFrameworkCore;
using OrderService.Model.Models;

namespace OrderService.Repository.Context;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{}

	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }
	public DbSet<Customer> Customers { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Order>(entity =>
		{
			entity.Property(o => o.Amount)
				.HasColumnType("decimal(18, 2)");

			entity.HasOne(o => o.Customer)
				.WithMany()
				.HasForeignKey(o => o.CustomerId)
				.IsRequired();
		});

		modelBuilder.Entity<OrderItem>(entity =>
		{
			entity.Property(o => o.Price)
				.HasColumnType("decimal(18, 2)");

			entity.HasOne(o => o.Order)
				.WithMany()
				.HasForeignKey(o => o.OrderId)
				.IsRequired();
		});

	}
}
