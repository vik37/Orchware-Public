using FileStorage;
using DbQueryBuilder;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Common.Middleware;
using Orchware.Frontoffice.API.Common.Pipeline;
using Orchware.Frontoffice.API.Infrastructure.Persistence;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;
using System.Reflection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

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
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLDbConnection"), sqlOptions =>
        sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
});

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

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

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
    await initializer.InitializeData(filePath);
}

app.Run();
