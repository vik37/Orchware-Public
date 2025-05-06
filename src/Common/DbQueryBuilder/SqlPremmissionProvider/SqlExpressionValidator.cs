namespace DbQueryBuilder.SqlPremmissionProvider;

public class SqlExpressionValidator
{
	private readonly IFieldPermissionProvider _permissionProvider;

	public SqlExpressionValidator(IFieldPermissionProvider permissionProvider)
	{
		_permissionProvider = permissionProvider;
	}

	public bool IsSafeExpression(string table, string? expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			return true;

		var forbiddenPatterns = new[] { ";", "--", "/*", "*/", "DROP", "DELETE", "INSERT", "UPDATE" };
		if (forbiddenPatterns.Any(p => expression.ToUpper().Contains(p)))
			return false;

		var allowedOperators = new[] { ">=", "<=", "!=", "=", "<", ">", "LIKE", "IN" };

		foreach (var op in allowedOperators.OrderByDescending(o => o.Length))
		{
			var parts = expression.Split(op, 2);
			if (parts.Length == 2)
			{
				var field = parts[0].Trim();
				var value = parts[1].Trim();

				if (_permissionProvider == null)
					throw new InvalidOperationException("FieldPermissionProvider is not configured.");

				if (_permissionProvider.IsAllowed(table, field) && !string.IsNullOrEmpty(value))
					return true;
			}
		}
		return false;
	}
}
