using FileStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

namespace Orchware.Backoffice.Infrastructure.Persistence;

public static class PersistenceServiceRegistration
{
	public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
	{
		var connection = configuration.GetConnectionString("MSSQLDbConnection");

		services.AddDbContext<OrchwareBackofficeDbContext>(opt =>
		{
			opt.UseSqlServer(configuration.GetConnectionString("MSSQLDbConnection"),
				options =>
				{
					options.EnableRetryOnFailure(
							maxRetryCount: 5,
							maxRetryDelay: TimeSpan.FromSeconds(5),
							errorNumbersToAdd: null);
				});
		});

		services.AddFileServices();
		services.AddScoped<OrchwareBackInitializer>();

		services.AddResiliencePipeline("orchware-backoffice-pipeline", builder =>
		{
			builder.AddRetry(new RetryStrategyOptions
				{
					ShouldHandle = new PredicateBuilder()
						.Handle<FileNotFoundException>()
						.Handle<DbUpdateException>()
						.Handle<Exception>(),

					MaxRetryAttempts = 5,
					Delay = TimeSpan.FromSeconds(5),
					BackoffType = DelayBackoffType.Constant,
					UseJitter = true,
					OnRetry = args =>
					{
						Console.WriteLine($"Retry attempt {args.AttemptNumber} due to: {args.Outcome.Exception?.Message}");
						return ValueTask.CompletedTask;
					}
				})
				.AddTimeout(TimeSpan.FromSeconds(10));
		});


		return services;
	}
}
