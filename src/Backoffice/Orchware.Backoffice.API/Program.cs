using Orchware.Backoffice.API.Configurations;
using Orchware.Backoffice.API.Pipelines.Middleware;
using Orchware.Backoffice.Application;
using Orchware.Backoffice.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) =>
							logger.WriteTo.Console()
									.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration)
				.AddPersistenceServices(builder.Configuration);

builder.Services.AddAuthRegistration(builder.Configuration)
				.AddCorsRegistration()
				.AddOpenTelemetryRegistration(builder.Configuration)
				.AddRateLimmiterRegistration()
				.AddSwaggerRegistration(builder.Configuration);		

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks().AddDbContextCheck<OrchwareBackofficeDbContext>(); ;

var app = builder.Build();

try
{
	app.UseMiddleware<ExceptionMiddleware>();

	app.UseHttpsRedirection();

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