namespace Utilities.Pagination;

public class RequestFilterPagination : RequestPagination
{
	public string OrderBy { get; set; } = string.Empty;
	public List<FilterKeyValue> MultyFilter { get; set; } = new List<FilterKeyValue>();
	public string OrderDirection { get; set; } = "ASC";
	public string Search { get; set; } = string.Empty;
}

public class FilterKeyValue
{
	public string Key { get; set; } = string.Empty;
	public string Condition { get; set; } = "=";
	public List<string> Values { get; set; } = new List<string>();
}
