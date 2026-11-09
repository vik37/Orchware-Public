using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Configurations;
using Orchware.Notification.API.Hubs;
using Orchware.Notification.API.Infrastructure.AzCosmos;
using Orchware.Notification.API.Infrastructure.Persistence;
using Orchware.Notification.API.Services;
using Orchware.Notification.API.Settings.Models;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) =>
							logger.WriteTo.Console()
									.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthRegistration(builder.Configuration)
				.AddCorsRegistration()
				.AddDatabaseRegistration(builder.Configuration)
				.AddMassTransitConfigurations(builder.Configuration)
				.AddSwaggerRegistration(builder.Configuration)
				.AddOpenTelemetryRegistration(builder.Configuration);

builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<INotifySender, EmailSender>();
builder.Services.AddScoped<IOrderCosmosRepository, OrderCosmosRepository>();

builder.Services.AddOptions<CosmosDBSettings>().Bind(builder.Configuration.GetSection(nameof(CosmosDBSettings)))
	.Validate(o =>
				!string.IsNullOrWhiteSpace(o.Endpoint) &&
				!string.IsNullOrWhiteSpace(o.Secret),
				"Cosmos configuration is invalid"
			).ValidateOnStart();

builder.Services.AddSingleton(sp =>
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

builder.Services.AddSingleton(x =>
	new BlobServiceClient(builder.Configuration["StorageConnectionString"]));


var app = builder.Build();

app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(opt =>
	{
		opt.OAuthClientId(builder.Configuration["Authentication:ClientId"]);
		opt.OAuthUsePkce();
	});
	app.UseCors("OrchwareNOPoliciesDev");
}
else
{
	app.UseCors("OrchwareNOPoliciesProd");
}

//app.UseHttpsRedirection();

//app.UseCors("SignalRCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");

app.MapControllers();

await ApplyMigrationsAsync(app);

app.Run();

async Task ApplyMigrationsAsync(IApplicationBuilder app)
{
	using var scope = app.ApplicationServices.CreateScope();
	using var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

	try
	{
		var pending = await dbContext.Database.GetPendingMigrationsAsync();
		if (pending.Any())
		{
			Log.Information("Database Migrations started applied");
			await dbContext.Database.MigrateAsync();
			Log.Information("Migrations applied successfully.");
		}
	}
	catch (Exception ex)
	{
		Log.Fatal(ex, "Migration failed. Check SQL Server connectivity.");
	}
}