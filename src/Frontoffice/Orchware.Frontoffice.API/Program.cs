using FluentValidation;
using MediatR;
using Orchware.Frontoffice.API.Common.Configurations;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.Filters;
using Orchware.Frontoffice.API.Common.Middleware;
using Orchware.Frontoffice.API.Common.Pipeline;
using Orchware.Frontoffice.API.Infrastructure.Cache;
using Orchware.Frontoffice.API.Infrastructure.Identity;
using Orchware.Frontoffice.API.Infrastructure.Persistence;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;
using Serilog;
using StackExchange.Redis;
using System.Reflection;
using System.Text.Json.Serialization;
using DbQueryBuilder;
using FileStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) =>
							logger.WriteTo.Console()
									.ReadFrom.Configuration(context.Configuration));

var env = Environment.CurrentDirectory;

// Add services to the container.

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var redisConnection = builder.Configuration["RedisConnection:RedisPort"];

builder.Services.AddStackExchangeRedisCache(opt =>
{
	opt.Configuration = redisConnection;
	opt.InstanceName = builder.Configuration["RedisConnection:RedisInstance"];
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
	_ => ConnectionMultiplexer.Connect(redisConnection!));

builder.Services.AddTransient<IRedisLockService, RedisLockService>();

builder.Services.AddMediatR(conf => conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services
	.AddAuthRegistration(builder.Configuration)
	.AddCorsRegistration()
	.AddDatabaseRegistration(builder.Configuration)
	.AddMassTransitConfigurations(builder.Configuration)
	.AddOpenTelemetryRegistration(builder.Configuration)
	.AddPollyRegistrations()
	.AddRateLimmiterRegistration()
	.AddSwaggerRegistration(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

builder.Services.AddFileServices();
builder.Services.AddDbQueryBuilder<OrchwareFrontofficeFieldPremmisionProvider>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IRedisDistributedCacheService, RedisDistributedCacheService>();

builder.Services.AddScoped<RequireCompanyFilter>();

builder.Services.AddScoped<OrchwareFrontInitializer>();

ILogger<Program>? logger = null;

var app = builder.Build();

try
{
	app.UseMiddleware<ExceptionMiddleware>();

	//app.UseHttpsRedirection();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI(opt =>
		{
			opt.OAuthClientId(builder.Configuration["Authentication:ClientId"]);
			opt.OAuthUsePkce();
		});
		app.UseCors("OrchwareFOPoliciesDev");
	}
	else
	{
		app.UseCors("OrchwareFOPoliciesProd");
	}

	app.UseSerilogRequestLogging();

	app.UseRateLimiter();

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapControllers();

	app.MapHealthChecks("/health");

	using (var scope = app.Services.CreateScope())
	{
		var provider = scope.ServiceProvider;
		var initializer = provider.GetRequiredService<OrchwareFrontInitializer>();

		string filePath = Path.Combine(app.Environment.ContentRootPath, "Files", "Product.csv");
		await initializer.InitializeData(filePath);
	}
	app.Run();
}
catch (Exception ex)
{
	logger?.LogCritical(ex, "Unhandled exception during ORCHWARE - FRONTOFFICE app startup.");
	throw;
}

