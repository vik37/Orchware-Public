using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orchware.Backoffice.API.Models;
using Orchware.Backoffice.API.Pipelines.Middleware;
using Orchware.Backoffice.Application;
using Orchware.Backoffice.Infrastructure.Persistence;
using Serilog;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) =>
							logger.WriteTo.Console()
											.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration)
				.AddPersistenceServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

	opt.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var otelSettings = builder.Configuration.GetSection("OpenTelemetry:OtlpExporter").Get<OtelSettings>();

var resourceBuilder = ResourceBuilder.CreateDefault().AddService("orchware-backoffice-api");

builder.Services.AddOpenTelemetry()
	.WithTracing(tracingProvider =>
	{
		tracingProvider.SetResourceBuilder(resourceBuilder)
		.AddAspNetCoreInstrumentation()
		.AddHttpClientInstrumentation()
		.AddOtlpExporter(otlp =>
		{
			otlp.Endpoint = new Uri(otelSettings!.Endpoint);
			otlp.Protocol = otelSettings!.Protocol.ToLower() == "grpc" ?
												OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf;
		});
	})
	.WithMetrics(matricsProvider =>
	{
		matricsProvider.SetResourceBuilder(resourceBuilder)
						.AddHttpClientInstrumentation()
						.AddHttpClientInstrumentation()
						.AddRuntimeInstrumentation()
						.AddOtlpExporter(otpl =>
						{
							otpl.Endpoint = new Uri(otelSettings!.Endpoint);
							otpl.Protocol = otelSettings.Protocol.ToLower() == "grpc" ?
												OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf;
						});
	});

builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = 429;
	options.AddPolicy("slide-by-ip", httpContext =>
		RateLimitPartition.GetSlidingWindowLimiter(
			partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
			factory: _ => new SlidingWindowRateLimiterOptions
			{
				PermitLimit = 400,
				Window = TimeSpan.FromMinutes(1),
				SegmentsPerWindow = 2,
				QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
				QueueLimit = 1
			}));
	options.OnRejected = (ctx, token) =>
	{
		var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
		logger.LogWarning("Rate limit triggered for {IpAddress}", ctx.HttpContext.Connection.RemoteIpAddress);
		return ValueTask.CompletedTask;
	};
});

builder.Services.AddCors(option =>
{
	option.AddPolicy("OrchwareBOPoliciesDev", policy =>
	{
		policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
			 .AllowAnyHeader()
			 .AllowAnyMethod()
			 .AllowCredentials();
	});
});


builder.Services.AddHealthChecks().AddDbContextCheck<OrchwareBackofficeDbContext>();

var app = builder.Build();

try
{
	app.UseMiddleware<ExceptionMiddleware>();

	app.UseHttpsRedirection();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseCors("OrchwareBOPoliciesDev");

	app.UseSerilogRequestLogging();

	app.UseRateLimiter();

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
	app.Logger.LogCritical(ex, "Unhandled exception during ORCHWARE - BACKOFFICE app startup.");
}