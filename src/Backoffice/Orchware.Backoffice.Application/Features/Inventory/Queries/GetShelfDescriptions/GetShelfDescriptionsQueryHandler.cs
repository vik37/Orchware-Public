using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;
using Orchware.Backoffice.Domain.Constants;

namespace Orchware.Backoffice.Application.Features.Inventory.Queries.GetShelfDescriptions;

public record GetShelfDescriptionQuery : IRequest<GetShelfDescriptionsResponse>;

public class GetShelfDescriptionsQueryHandler : IRequestHandler<GetShelfDescriptionQuery, GetShelfDescriptionsResponse>
{
	private readonly string _connectionString;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private string Schema = $"[{DomainSchema.Inventory}]";

	public GetShelfDescriptionsQueryHandler(IOptions<DatabaseConnectionOption> dbConnection,
		ISqlQueryBuilder sqlQueryBuilder)
	{
		_connectionString = dbConnection.Value.MSSQLDbConnection;
		_sqlQueryBuilder = sqlQueryBuilder;
	}

	public async Task<GetShelfDescriptionsResponse> Handle(GetShelfDescriptionQuery request, 
		CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new GetShelfDescriptionsResponse(0);

		var sql = _sqlQueryBuilder.Select("s.Id, s.Code, s.SeasonalFruits, " +
							"p.Id, p.Name, p.AvailableQuantity, p.Units, p.ShelfId")
					.From($"{Schema}.[Shelf] s")
					.Join("LEFT", $"{Schema}.[Product] p", "p.ShelfId = s.Id")
					.OrderBy("s.Id")
					.Build();

		try
		{
			using var connection = new SqlConnection(_connectionString);

			var shelfDictionary = new Dictionary<int, ShelfDto>();

			var shelves = (await connection.QueryAsync<ShelfDto, ProductDto, ShelfDto>(
				sql,
				(shelf, product) =>
				{
					if (!shelfDictionary.TryGetValue(shelf.Id, out var shelfEntry))
					{
						shelfEntry = shelf;
						shelfDictionary[shelf.Id] = shelfEntry;
					}

					if (product is not null)
						shelfEntry.AddProducts(product);

					return shelfEntry;
				},
				splitOn: "Id")).Distinct();

			return new GetShelfDescriptionsResponse(
						shelves.Sum(s => s.Products?.Count() ?? 0))
			{
				Shelves = shelves
			};
		}
		catch (Exception)
		{
			throw;
		}
	}
}
