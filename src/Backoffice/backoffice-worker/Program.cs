using Orchware.Backoffice.Infrastructure;
using backoffice_worker.Configurations;
using backoffice_worker.Services;
using Serilog;

//var builder = Host.CreateApplicationBuilder(args);

//builder.Logging.ClearProviders();

//builder.Services.AddSerilog((services, loggerConfiguration) =>
//{
//	loggerConfiguration
//		.ReadFrom.Configuration(builder.Configuration)
//		.ReadFrom.Services(services)
//		.Enrich.FromLogContext();
//});

//builder.Services.AddInfrastructureServices(builder.Configuration);

//builder.Services.AddOpenTelemetryRegistration(builder.Configuration);

//builder.Services.AddCronJob<BillingTaskService>(c =>
//{
//	c.CronExpression = "0 */1 * * * *";
//	c.TimeZoneInfo = TimeZoneInfo.Local;
//});

//try
//{
//	Log.Information("Starting ORCHWARE BACKOFFICE BACKGROUND WORKER");

//	var host = builder.Build();
//	await host.RunAsync();
//}
//catch (Exception ex)
//{
//	Log.Fatal(ex, "Application failed to start");
//}
//finally
//{
//	Log.CloseAndFlush();
//}


Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

try
{
	Log.Information("Starting ORCHWARE BACKOFFICE BACKGROUND WORKER...");

	var builder = Host.CreateApplicationBuilder(args);

	builder.Logging.ClearProviders();

	builder.Services.AddSerilog((services, loggerConfiguration) =>
	{
		loggerConfiguration
			.ReadFrom.Configuration(builder.Configuration)
			.ReadFrom.Services(services)
			.Enrich.FromLogContext();

		var seqUrl = builder.Configuration["SEQ_URL"] ?? Environment.GetEnvironmentVariable("SEQ_URL");

		if (!string.IsNullOrEmpty(seqUrl))
			loggerConfiguration.WriteTo.Seq(seqUrl);
	});

	builder.Services.AddInfrastructureServices(builder.Configuration);

	builder.Services.AddOpenTelemetryRegistration(builder.Configuration);

	builder.Services.AddCronJob<BillingTaskService>(c =>
	{
		c.CronExpression = "0 */1 * * * *";
		c.TimeZoneInfo = TimeZoneInfo.Local;
	});

	var host = builder.Build();

	Log.Information("Host built successfully. Running host...");
	await host.RunAsync();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Worker Host terminated unexpectedly during startup!");
	Console.WriteLine($"[CRITICAL ERROR STACKTRACE]: {ex}");
}
finally
{
	Log.CloseAndFlush();
}