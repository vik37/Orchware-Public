using Orchware.Frontoffice.API.Common.Pagginations;
using System.Text.RegularExpressions;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

public static class SqlFilterHelper
{
	private static readonly string[] AllowedConditions = { "=", "!=", "<", ">", "<=", ">=", "LIKE", "IN" };

	public static string BuildSafeWhereClause(List<FilterKeyValue> filters)
	{
		if (filters == null || !filters.Any())
			return string.Empty;

		var clauses = new List<string>();

		foreach (var filter in filters)
		{
			if (string.IsNullOrWhiteSpace(filter.Key) || filter.Values == null || filter.Values.Count == 0)
				continue;

			var condition = filter.Condition?.ToUpper().Trim() ?? "=";

			if (!AllowedConditions.Contains(condition))
				continue;

			var sanitizedKey = Sanitize(filter.Key);

			if (condition == "IN")
			{
				var inValues = string.Join(", ", filter.Values.Select(v => $"'{v.Replace("'", "''")}'"));
				clauses.Add($"{sanitizedKey} IN ({inValues})");
			}
			else if (condition == "LIKE")
			{
				foreach (var value in filter.Values)
					clauses.Add($"{sanitizedKey} LIKE '{value.Replace("'", "''")}'");
			}
			else
			{
				foreach (var value in filter.Values)
					clauses.Add($"{sanitizedKey} {condition} '{value.Replace("'", "''")}'");
			}
		}
		return string.Join(" AND ", clauses);
	}

	private static string Sanitize(string input)
		=> Regex.Replace(input, @"[^\w\.]", string.Empty); 
}
