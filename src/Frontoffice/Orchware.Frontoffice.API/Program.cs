using FileStorage;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Infrastructure.Persistence;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var env = Environment.CurrentDirectory;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DapperContext>();

builder.Services.AddDbContext<OrchwareDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"), sqlOptions =>
        sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
});

builder.Services.AddFileServices();

builder.Services.AddScoped<OrchwareFrontInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using(var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;
    var initializer = provider.GetRequiredService<OrchwareFrontInitializer>();

    string filePath = Path.Combine(app.Environment.ContentRootPath, "Files", "Product.csv");
	try
	{
		await initializer.InitializeData(filePath);
	}
	catch (Exception ex)
	{
		app.Logger.LogError(ex, "Failed to initialize product data.");
	}
}

app.Run();
