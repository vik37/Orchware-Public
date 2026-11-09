using Azure.Storage.Blobs;
using FluentValidation;
using MediatR;
using Orchware.Backoffice.API.Configurations;
using Orchware.Backoffice.API.Pipelines.Behaviors;
using Orchware.Backoffice.API.Pipelines.Middleware;
using Orchware.Backoffice.Application;
using Orchware.Backoffice.Application.Features.Orders.Commands.FilterHistoryOrder;
using Orchware.Backoffice.Infrastructure;
using Orchware.Backoffice.Infrastructure.AZCosmosDB.Settings;
using Orchware.Backoffice.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) =>
							logger.WriteTo.Console()
									.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration)
				.AddInfrastructureServices(builder.Configuration)
				.AddInfrastructureForAPIService();

builder.Services.AddAuthRegistration(builder.Configuration)
				.AddCorsRegistration()
				.AddMassTransitRegistration(builder.Configuration)
				.AddOpenTelemetryRegistration(builder.Configuration)
				.AddRateLimmiterRegistration()
				.AddSwaggerRegistration(builder.Configuration);

var applicationAssembly = typeof(FilterHistoryOrderValidator).Assembly;
builder.Services.AddValidatorsFromAssembly(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks().AddDbContextCheck<OrchwareBackofficeDbContext>();

builder.Services.AddOptions<CosmosDBSettings>().Bind(builder.Configuration.GetSection(nameof(CosmosDBSettings)))
	.Validate(o =>
				!string.IsNullOrWhiteSpace(o.Endpoint) &&
				!string.IsNullOrWhiteSpace(o.Secret),
				"Cosmos configuration is invalid"
			)
			.ValidateOnStart();

builder.Services.AddSingleton(x =>
	new BlobServiceClient(builder.Configuration["StorageConnectionString"]));

var app = builder.Build();

try
{
	app.UseMiddleware<ExceptionMiddleware>();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI(opt =>
		{
			opt.OAuthClientId(builder.Configuration["Authentication:ClientId"]);
			opt.OAuthClientSecret(builder.Configuration["Authentication:ClientSecrets"]);
		});
		app.UseCors("OrchwareBOPoliciesDev");
	}
	else
	{
		app.UseCors("OrchwareBOPoliciesProd");
	}

	app.UseSerilogRequestLogging();

	app.UseRateLimiter();

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapControllers();

	app.MapHealthChecks("/health");

	using (var scope = app.Services.CreateScope())
	{
		var initializer = scope.ServiceProvider.GetRequiredService<OrchwareBackInitializer>();
		await initializer.InitializeData();
	}

	app.Run();
}
catch (Exception ex)
{
	var logger = app.Services.GetRequiredService<ILogger<Program>>();
	logger.LogCritical(ex, "Unhandled exception during ORCHWARE - BACKOFFICE app startup.");
}