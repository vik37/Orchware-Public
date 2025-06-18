namespace Orchware.Frontoffice.API.Common.SQLScriptBuilder;

public interface ISqlQueryBuilder
{
	ISqlQueryBuilder Select(string columns);
	ISqlQueryBuilder From(string table);
	ISqlQueryBuilder Where(string condition);
	ISqlQueryBuilder OrderBy(string sorting, bool ascending = true);

	ISqlQueryBuilder Join(string joinType, string table, string onCondition);
	ISqlQueryBuilder GroupBy(string columns);

	string Build(bool autoRestore = true);
}
