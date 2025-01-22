using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Domain;
using PaymentService.Domain.Interface;
using PaymentService.Model.Models;
using PaymentService.Repository.Context;
using PaymentService.Repository.Interface;
using PaymentService.Repository.Repository;
using PyamentService.Api.Filter;
using Stripe;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("ConnectionDefault")
                       ?? throw new Exception("Connection string 'ConnectionDefault' is not configured or is missing.");
var mySqlVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseMySql(connectionString, mySqlVersion); });

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentDomain, PaymentDomain>();
builder.Services.AddSingleton<IEventPublisher, EventPublisher>();
builder.Services.AddLogging();

var stripeSettings = builder.Configuration.GetSection("Stripe");
StripeConfiguration.ApiKey = stripeSettings["SecretKey"];

builder.Services.AddControllers(options =>
	options.Filters.Add<GlobalExceptionFilter>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
