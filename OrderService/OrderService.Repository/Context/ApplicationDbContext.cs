using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderService.Model.Enum;
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
			entity.HasOne(o => o.Customer)
				.WithMany()
				.HasForeignKey(o => o.CustomerId)
				.IsRequired();
		});

		modelBuilder.Entity<OrderItem>(entity =>
		{
			entity.HasOne(o => o.Order)
				.WithMany()
				.HasForeignKey(o => o.OrderId)
				.IsRequired();
		});

		modelBuilder.Entity<Order>(entity =>
		{
			entity.Property(p => p.OrderStatus)
				.HasConversion(new EnumToStringConverter<OrderStatus>());
		});
	}
}
