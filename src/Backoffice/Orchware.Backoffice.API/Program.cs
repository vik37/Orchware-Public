using Orchware.Backoffice.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPersistenceServices(builder.Configuration);

//builder.Services.AddDbContext<OrchwareBackofficeDbContext>(opt =>
//{
//	opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLDbConnection"));
//});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
	var initializer = scope.ServiceProvider.GetRequiredService<OrchwareBackInitializer>();
	await initializer.InitializeData();
}

app.Run();
