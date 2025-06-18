using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;

namespace Orchware.Frontoffice.API.Common.Configurations;

public static class PollyConfigurations
{
	public static IServiceCollection AddPollyRegistrations(this IServiceCollection services)
	{
		services.AddResiliencePipeline("orchware-frontoffice-pipeline", builder =>
		{
			builder.AddRetry(new RetryStrategyOptions
			{
				ShouldHandle = new PredicateBuilder()
						.Handle<DbUpdateException>()
						.Handle<Exception>(),

				MaxRetryAttempts = 5,
				Delay = TimeSpan.FromSeconds(5),
				BackoffType = DelayBackoffType.Constant,

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
