namespace Orchware.Frontoffice.API.Common.SQLScriptBuilder;

public interface ISqlQueryBuilder
{
	ISqlQueryBuilder Select(string columns);
	ISqlQueryBuilder From(string table);
	ISqlQueryBuilder Where(string condition);
	ISqlQueryBuilder OrderBy(string sorting, bool ascending = true);
	string Build();
}
