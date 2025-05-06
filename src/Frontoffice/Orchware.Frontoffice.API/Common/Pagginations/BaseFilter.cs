namespace Orchware.Frontoffice.API.Common.Pagginations;

public class BaseFilter
{
	public int PageIndex { get; set; }
	public int PageSize { get; set; }	
}

public class RequestFilterPaggination : BaseFilter
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

public class ResponseFilterPaggination<T> : BaseFilter
{
	public int TotalCount { get; set; }
	public int TotalPages => (int)Math.Ceiling((double) TotalCount / PageSize);
	public T? Data { get; set; }
}