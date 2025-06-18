using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Orchware.Backoffice.Infrastructure.Persistence;

public class OrchwareBackofficeDbContextFactory : IDesignTimeDbContextFactory<OrchwareBackofficeDbContext>
{
	public OrchwareBackofficeDbContext CreateDbContext(string[] args)
	{
		var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
		var basePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\Orchware.Backoffice.API");

		Console.WriteLine($"Environment: {environment}");

		IConfigurationRoot configuration = new ConfigurationBuilder()
			.SetBasePath(basePath)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"appsettings.{environment}.json", optional: true)
			.Build();

		var connectionString = configuration.GetConnectionString("MSSQLDbConnection");

		var optionsBuilder = new DbContextOptionsBuilder<OrchwareBackofficeDbContext>();
		optionsBuilder.UseSqlServer(connectionString);

		return new OrchwareBackofficeDbContext(optionsBuilder.Options);
	}
}
