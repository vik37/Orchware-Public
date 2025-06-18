using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Inventory.Shared.Models;
using Orchware.Backoffice.Application.Features.Shared.Enums;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;

namespace Orchware.Backoffice.Application.Features.Inventory.Queries.GetShelvesBySeason;

public record GetShelvesBySeasonQuery(SeasonalFruits SeasonalFruits) : IRequest<List<ShelfDto>>;

public class GetShelvesBySeasonQueryHandler : IRequestHandler<GetShelvesBySeasonQuery, List<ShelfDto>>
{
	private readonly string _connectionString;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private string Schema = $"[{DomainSchema.Inventory}]";

	public GetShelvesBySeasonQueryHandler(IOptions<DatabaseConnectionOption> dbOption, 
		ISqlQueryBuilder sqlQueryBuilder)
	{
		_connectionString = dbOption.Value.MSSQLDbConnection;
		_sqlQueryBuilder = sqlQueryBuilder;
	}

	public async Task<List<ShelfDto>> Handle(GetShelvesBySeasonQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new List<ShelfDto>();

		var queryShelves = _sqlQueryBuilder.Select("Id,Code,SeasonalFruits")
											.From($"{Schema}.[Shelf]")
											.Where("SeasonalFruits = @SeasonalFruits")
											.OrderBy("Id").Build();

		try
		{
			using var connection = new SqlConnection(_connectionString);

			return (await connection.QueryAsync<ShelfDto>(queryShelves,
								new { SeasonalFruits = (int)request.SeasonalFruits }))
							.ToList();
		}
		catch (Exception)
		{
			throw;
		}
	}
}
