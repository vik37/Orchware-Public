using FileStorage;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Shared.Contract.FileServices;
using Orchware.Backoffice.Application.Features.Shared.Contract.Identity;
using Orchware.Backoffice.Application.Features.Shared.Contract.IntegrationEventProducers;
using Orchware.Backoffice.Domain.Entities.Inventory;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Entities.Proccessing;
using Orchware.Backoffice.Domain.Entities.Settings;
using Orchware.Backoffice.Domain.Primitives;
using Orchware.Backoffice.Infrastructure.AZCosmosDB;
using Orchware.Backoffice.Infrastructure.AZCosmosDB.Abstraction;
using Orchware.Backoffice.Infrastructure.AZCosmosDB.Settings;
using Orchware.Backoffice.Infrastructure.Cache;
using Orchware.Backoffice.Infrastructure.Files;
using Orchware.Backoffice.Infrastructure.Files.FileTypes;
using Orchware.Backoffice.Infrastructure.Identity.Services;
using Orchware.Backoffice.Infrastructure.IntegrationEvents.EventPublisher;
using Orchware.Backoffice.Infrastructure.Persistence;
using Orchware.Backoffice.Infrastructure.Persistence.Repository;
using Polly;
using Polly.Retry;
using StackExchange.Redis;

namespace Orchware.Backoffice.Infrastructure;

public static class InfrastructureServiceRegistration
{
	/// <summary>
	///  Global DI Registretion for Infrastructure Services. (API, Backoffice Worker Services)
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	/// <returns>Extended Collection of Registered Services.</returns>
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{

		#region Database Context Register
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
		#endregion

		#region Redis

		var redisConnection = configuration["RedisConnection:RedisPort"];

		services.AddStackExchangeRedisCache(opt =>
		{
			opt.Configuration = redisConnection;
			opt.InstanceName = configuration["RedisConnection:RedisInstance"];
		});

		services.AddSingleton<IConnectionMultiplexer>(
			_ => ConnectionMultiplexer.Connect(redisConnection!));
		#endregion

		#region Scoped Services

		services.AddScoped<IRedisDistributedCacheService, RedisDistributedCacheService>();
		services.AddScoped<IOrderRepository, OrderRepository>();
		services.AddScoped<IShelfRepository, ShelfRepository>();
		services.AddScoped<IPaymentRulesRepository, PaymentRulesRepository>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		services.AddScoped<IBilingTaskRepository, BilingTaskRepository>();
		services.AddScoped<IOrderCosmosRepository,OrderCosmosRepository>();
		#endregion


		#region Azure CosmosDB Register

		services.AddOptions<CosmosDBSettings>()
			.Bind(configuration.GetSection(nameof(CosmosDBSettings)))
			.Validate(o =>
				!string.IsNullOrWhiteSpace(o.Endpoint) &&
				!string.IsNullOrWhiteSpace(o.Secret),
				"Cosmos configuration is invalid"
			)
			.ValidateOnStart();

		services.AddSingleton(sp =>
		{
			var options = sp.GetRequiredService<IOptions<CosmosDBSettings>>().Value;

			var cleanEndpoint = options.Endpoint?.Trim().Replace("\r", "").Replace("\n", "");
			var rawSecret = options.Secret?.Trim().Replace("\r", "").Replace("\n", "");

			if (!string.IsNullOrEmpty(rawSecret))
			{
				rawSecret = rawSecret.Trim('\'').Trim('"');

				if (rawSecret.Length % 4 != 0)
					rawSecret = rawSecret.PadRight(rawSecret.Length + (4 - rawSecret.Length % 4), '=');
			}

			return new CosmosClient(cleanEndpoint, rawSecret, new CosmosClientOptions
			{
				ConnectionMode = ConnectionMode.Direct
			});
		});

		#endregion

		return services;
	}

	/// <summary>
	///  Global DI Registretion for Infrastructure Services Intended only for API Service. 
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	/// <returns>Extended Collection of Registered Services.</returns>
	public static IServiceCollection AddInfrastructureForAPIService(this IServiceCollection services) 
	{
		#region Database Initializer

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

		#endregion

		#region File Storage
		services.AddKeyedTransient<IFileStorageService, LocalFileStorageService>("local");
		services.AddKeyedTransient<IFileStorageService, AzureBlobFileStorage>("azure");

		services.AddSingleton<IFileStorageFactory, FileStorageFactory>();
		#endregion

		#region Scoped Services

		services.AddScoped<IUserContextService, UserContextService>();
		services.AddScoped<IIntegrationEventProducer, IntegrationEventProducer>();
		services.AddScoped<IRedisLockService, RedisLockService>();

		#endregion

		return services;
	}
}
