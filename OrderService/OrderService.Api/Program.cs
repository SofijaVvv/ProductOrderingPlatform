using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OrderService.Api.Extentions;
using OrderService.Api.Filter;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Event;
using OrderService.Repository.Context;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options =>
	options.Filters.Add<GlobalExceptionFilter>()).AddJsonOptions(options =>
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ConnectionDefault")
                       ?? throw new Exception("Connection string 'ConnectionDefault' is not configured or is missing.");
var mySqlVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseMySql(connectionString, mySqlVersion); });
builder.AddRepositories();
builder.AddDomains();
builder.AddServices();
builder.AddHttpClient();
builder.Services.AddSingleton<EventSubscriber>();
builder.Services.AddHostedService<EventBusSubscriberService>();


builder.Services.AddLogging();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
