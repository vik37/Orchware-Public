namespace DbQueryBuilder.SqlPremmissionProvider;

public interface IFieldPermissionProvider
{
	bool IsAllowed(string table, string field);
}
