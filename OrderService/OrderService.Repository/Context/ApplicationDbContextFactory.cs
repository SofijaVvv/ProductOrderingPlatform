using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OrderService.Repository.Context;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	private const string ErrorMessage = "Connection string is not configured properly or is missing.";
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

		var configuration = new ConfigurationBuilder()
			.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OrderService.Api"))
			.AddJsonFile("appsettings.json", false)
			.Build();

		var connectionString = configuration.GetConnectionString("ConnectionDefault");
		var mySqlVersion = ServerVersion.AutoDetect(connectionString);
		optionsBuilder.UseMySql(connectionString ?? throw new InvalidOperationException(ErrorMessage), mySqlVersion);

		return new ApplicationDbContext(optionsBuilder.Options);
	}
}
