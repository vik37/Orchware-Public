using Orchware.Backoffice.API.Pipelines.Middleware;
using Orchware.Backoffice.Application;
using Orchware.Backoffice.Infrastructure.Persistence;
using Serilog;
using System.Reflection;

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

	app.UseCors("OrchwareBOPoliciesDev");

	app.UseSerilogRequestLogging();

	app.UseHttpsRedirection();

	app.UseAuthorization();

	app.MapControllers();

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
	logger.LogError(ex, "Unhandled exception during ORCHWARE - BACKOFFICE app startup.");
}