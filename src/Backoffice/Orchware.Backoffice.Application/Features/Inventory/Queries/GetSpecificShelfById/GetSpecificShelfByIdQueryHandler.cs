using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Inventory.Shared.Models;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;

namespace Orchware.Backoffice.Application.Features.Inventory.Queries.GetSpecificShelfById;

public record GetSpecificShelfByIdQuery(int id) : IRequest<ShelfDto>;

public class GetSpecificShelfByIdQueryHandler : IRequestHandler<GetSpecificShelfByIdQuery, ShelfDto>
{
	private readonly string _connectionString;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private string Schema = "[inventory]";

	public GetSpecificShelfByIdQueryHandler(IOptions<DatabaseConnectionOption> dbConnection, 
		ISqlQueryBuilder sqlQueryBuilder)
	{
		_connectionString = dbConnection.Value.MSSQLDbConnection;
		_sqlQueryBuilder = sqlQueryBuilder;
	}

	public async Task<ShelfDto> Handle(GetSpecificShelfByIdQuery request, CancellationToken cancellationToken)
	{
		if(cancellationToken.IsCancellationRequested)
			return new ShelfDto();

		var query = $@"{_sqlQueryBuilder.Select("Id, Code, SeasonalFruits")
											.From($"{Schema}.[Shelf]")
											.Where("Id = @Id").Build()}

		              {_sqlQueryBuilder.Select("Id, [Name], AvailableQuantity, Price, MinQuantity, Units, [Image]," +
														"ShelfId")
												.From($"{Schema}.[Product]")
												.Where("ShelfId = @Id")
												.Build()}";

		try
		{
			using var connection = new SqlConnection(_connectionString);
			using var multi = await connection.QueryMultipleAsync(query, param: new { request.id });

			var shelf = await multi.ReadFirstOrDefaultAsync<ShelfDto>();

			if (shelf is null)
				throw new NotFoundException("There is no such Shelf in our Warehouse.");

			shelf.AddRangeOfProducts(await multi.ReadAsync<ProductDto>());

			return shelf;
		}
		catch (Exception)
		{
			throw;
		}
	}
}
