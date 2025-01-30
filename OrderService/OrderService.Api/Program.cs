using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Domain;
using OrderService.Domain.Interface;
using OrderService.Repository.Context;
using OrderService.Repository.Interface;
using OrderService.Repository.Repository;
using OrderService.Service.Event;
using OrderService.Service.Interface;
using OrderService.Service.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ConnectionDefault")
                       ?? throw new Exception("Connection string 'ConnectionDefault' is not configured or is missing.");
var mySqlVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseMySql(connectionString, mySqlVersion); });

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<ICustomerDomain, CustomerDomain>();
builder.Services.AddScoped<IOrderPaymentDomain, OrderPaymentDomain>();
builder.Services.AddScoped<IOrderDomain, OrderDomain>();
builder.Services.AddScoped<IOrderItemDomain, OrderItemDomain>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSingleton<EventSubscriber>();
builder.Services.AddHostedService<EventBusSubscriberService>();
builder.Services.AddHttpClient();
builder.Services.AddLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
