using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductService.Api.Filter;
using ProductService.Domain;
using ProductService.Domain.Interfaces;
using ProductService.Model.MongoDbModel;
using ProductService.Repository;
using ProductService.Repository.Db;
using ProductService.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbConnection"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
	var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
	return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
	var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
	var client = sp.GetRequiredService<IMongoClient>();
	return client.GetDatabase(settings.DatabaseName);
});
builder.Services.AddSingleton<MongoDataValidation>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductDomain, ProductDomain>();

builder.Services.AddControllers(options =>
	options.Filters.Add<GlobalExceptionFilter>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var mongoDbService = app.Services.GetRequiredService<MongoDataValidation>();
mongoDbService.SetCollectionValidation();
mongoDbService.TestInsertProduct();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
//
app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
