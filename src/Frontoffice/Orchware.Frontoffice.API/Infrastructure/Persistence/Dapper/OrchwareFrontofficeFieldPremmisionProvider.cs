using DbQueryBuilder.SqlPremmissionProvider;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

public class OrchwareFrontofficeFieldPremmisionProvider : IFieldPermissionProvider
{
	private readonly Dictionary<string, HashSet<string>> _allowedFieldsByTable = new()
	{
		["Product"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Id", "Name", "Price", "SeasonalFruits", "Units" },
		["Order"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Status", "CreatedAt" }
	};

	public bool IsAllowed(string table, string field)
		=> _allowedFieldsByTable.TryGetValue(table, out var fields) && fields.Contains(field);
}
