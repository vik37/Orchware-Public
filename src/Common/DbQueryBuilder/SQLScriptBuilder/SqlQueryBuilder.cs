using System.Text;

namespace Orchware.Frontoffice.API.Common.SQLScriptBuilder;

public class SqlQueryBuilder : ISqlQueryBuilder
{
	private readonly StringBuilder _queryBuilder = new StringBuilder();
	private readonly List<string> _columns = new List<string>();
	private bool _fromSet = false;
	private string _orderByClause = "";
	private string _groupByClause = "";

	public ISqlQueryBuilder Select(string columns)
	{
		if (columns.Trim() == "*")
		{
			_columns.Clear();
			_columns.Add("*");
		}
		else if (!_columns.Contains("*") && !_columns.Contains(columns))
		{
			_columns.Add(columns);
		}
		return this;
	}

	public ISqlQueryBuilder From(string table)
	{
		if (_fromSet)
			return this;

		_queryBuilder.AppendLine($"FROM {table}");
		_fromSet = true;

		return this;
	}

	public ISqlQueryBuilder Join(string joinType, string table, string onCondition)
	{
		if (string.IsNullOrWhiteSpace(joinType) || string.IsNullOrWhiteSpace(table) || string.IsNullOrWhiteSpace(onCondition))
			return this;

		joinType = joinType.Trim().ToUpper();
		if (joinType != "INNER" && joinType != "LEFT" && joinType != "RIGHT")
			throw new ArgumentException("Join type must be INNER, LEFT, or RIGHT.");

		_queryBuilder.AppendLine($"{joinType} JOIN {table} ON {onCondition}");	
		return this;
	}

	public ISqlQueryBuilder Where(string condition)
	{
		if (!string.IsNullOrWhiteSpace(condition))
		{
			if (_queryBuilder.ToString().Contains("WHERE"))
				_queryBuilder.AppendLine($"AND {condition}");
			else
				_queryBuilder.AppendLine($"WHERE {condition}");
		}
		return this;
	}

	public ISqlQueryBuilder OrderBy(string sorting, bool ascending = true)
	{
		if (!string.IsNullOrWhiteSpace(sorting))
		{
			_orderByClause = $"ORDER BY {sorting} {(ascending ? "ASC" : "DESC")}";
		}
		return this;
	}

	public ISqlQueryBuilder GroupBy(string columns)
	{
		if (!string.IsNullOrWhiteSpace(columns))
		{
			_groupByClause = $"GROUP BY {columns}";
		}
		return this;
	}

	public string Build(bool autoRestore = true)
	{
		var result = $"SELECT {string.Join(", ", _columns)} {Environment.NewLine}{_queryBuilder.ToString().Trim()} {(!string.IsNullOrEmpty(_groupByClause)?$"{Environment.NewLine}{_groupByClause}":"")} {Environment.NewLine}{_orderByClause}".Trim();

		if(autoRestore)
			Restore();

		return result;
	}

	private void Restore()
	{
		_queryBuilder.Clear();
		_columns.Clear();
		_fromSet = false;
		_orderByClause = string.Empty;
		_groupByClause = string.Empty;
	}
}
