using Microsoft.Data.SqlClient;
using System.Data;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

public class DapperContext : IDisposable
{
	private readonly IDbConnection _connection;

	public DapperContext(IConfiguration configuration)
	{
		_connection = new SqlConnection(configuration.GetConnectionString("DbConnection"));
	}

	public IDbConnection CreateConnection() => _connection;

	public void Dispose()
	{
		_connection.Dispose();
	}
}
