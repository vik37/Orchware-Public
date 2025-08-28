using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

public class DapperContext
{
	private readonly string _connectionString;

	public DapperContext(IConfiguration configuration)
	{
		_connectionString = configuration.GetConnectionString("MSSQLDbConnection")??throw new ArgumentNullException();
	}

	public IDbConnection CreateConnection()
	{
		var connection = new SqlConnection(_connectionString);
		return connection;
	}

	public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(
		string storedProcedure,
		DynamicParameters parameters)
	{
		using var connection = CreateConnection();
		return await connection.QueryAsync<T>(
			storedProcedure,
			parameters,
			commandType: CommandType.StoredProcedure);
	}

	public async Task<(int totalCount, List<T> results)> ExecutePagedProcedureAsync<T>(
		string storedProcedure,
		DynamicParameters parameters,
		string totalCountField = "TotalCount")
	{
		using var connection = CreateConnection();
		using var multi = await connection.QueryMultipleAsync(
			storedProcedure,
			parameters,
			commandType: CommandType.StoredProcedure);

		int totalCount = await multi.ReadFirstAsync<int>();

		var data = (await multi.ReadAsync<T>()).ToList();

		return (totalCount, data);
	}
}
