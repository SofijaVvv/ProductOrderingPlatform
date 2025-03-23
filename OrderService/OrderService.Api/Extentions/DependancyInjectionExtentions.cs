using OrderService.Domain.Domain;
using OrderService.Domain.Interface;
using OrderService.Repository.Interface;
using OrderService.Repository.Repository;
using OrderService.Service.Interface;
using OrderService.Service.Services;

namespace OrderService.Api.Extentions;

public static class DependancyInjectionExtentions
{
	public static void AddDomains(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<ICustomerDomain, CustomerDomain>();
		builder.Services.AddScoped<IOrderDomain, OrderDomain>();
		builder.Services.AddScoped<IOrderItemDomain, OrderItemDomain>();
	}

	public static void AddRepositories(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
		builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
		builder.Services.AddScoped<IOrderRepository, OrderRepository>();
		builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
		builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
	}

	public static void AddServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IProductService, ProductService>();
		builder.Services.AddScoped<IPaymentService, PaymentService>();
	}

	public static void AddHttpClient(this WebApplicationBuilder builder)
	{
		builder.Services.AddHttpClient();
		builder.Services.AddHttpClient<IPaymentService, PaymentService>((sp, client) =>
		{
			var config = sp.GetRequiredService<IConfiguration>();
			var baseUrl = config.GetValue<string>("ApiSettings:PaymentBaseUrl")
			              ?? throw new Exception("PaymentBaseUrl in ApiSettings is not configured or is missing.");
			client.BaseAddress = new Uri(baseUrl);
		});

		builder.Services.AddHttpClient<IProductService, ProductService>((sp, client) =>
		{
			var config = sp.GetRequiredService<IConfiguration>();
			var baseUrl = config.GetValue<string>("ApiSettings:ProductBaseUrl")
			              ?? throw new Exception("ProductBaseUrl in ApiSettings is not configured or is missing.");
			client.BaseAddress = new Uri(baseUrl);
		});
	}
}
