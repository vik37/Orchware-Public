using DbQueryBuilder;
using FileStorage;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orchware.Frontoffice.API.Common.Configurations;
using Orchware.Frontoffice.API.Common.Middleware;
using Orchware.Frontoffice.API.Common.Models;
using Orchware.Frontoffice.API.Common.Pipeline;
using Orchware.Frontoffice.API.Infrastructure.Persistence;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;
using Serilog;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) => 
							logger.WriteTo.Console()
											.ReadFrom.Configuration(context.Configuration));

var env = Environment.CurrentDirectory;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DapperContext>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(conf => conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddDbContext<OrchwareDbContext>(opt =>
{
	var connectionString = builder.Configuration.GetConnectionString("MSSQLDbConnection")
		?? throw new InvalidOperationException("Connection string not found!");

	opt.UseSqlServer(connectionString, sqlOptions =>
	{
		sqlOptions.EnableRetryOnFailure(
			maxRetryCount: 5,
			maxRetryDelay: TimeSpan.FromSeconds(5),
			errorNumbersToAdd: null
		);

		sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
	});
});

builder.Services.AddPollyRegistrations();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidatorBehavior<,>));

builder.Services.AddFileServices();
builder.Services.AddDbQueryBuilder<OrchwareFrontofficeFieldPremmisionProvider>();

builder.Services.AddScoped<OrchwareFrontInitializer>();

builder.Services.AddSwaggerGen(opt =>
{
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

	opt.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var otelSettings = builder.Configuration.GetSection("OpenTelemetry:OtlpExporter").Get<OtelSettings>();

var resourceBuilder = ResourceBuilder.CreateDefault().AddService("orchware-frontoffice-api");

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
	options.AddPolicy("fixed-by-ip", httpContext =>
		RateLimitPartition.GetFixedWindowLimiter(
			partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
			factory: _ => new FixedWindowRateLimiterOptions
			{
				PermitLimit = 10,
				Window = TimeSpan.FromMinutes(1)
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
	option.AddPolicy("OrchwareFOPoliciesDev", policy =>
	{
		policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
			 .AllowAnyHeader()
			 .AllowAnyMethod()
			 .AllowCredentials();
	});
});

builder.Services.AddHealthChecks().AddDbContextCheck<OrchwareDbContext>();

var app = builder.Build();

try
{
	app.UseMiddleware<ExceptionMiddleware>();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseCors("OrchwareFOPoliciesDev");

	app.UseSerilogRequestLogging();

	app.UseHttpsRedirection();

	app.UseRateLimiter();

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
	app.Logger.LogError(ex, "Unhandled exception during ORCHWARE - FRONTOFFICE app startup.");
}
