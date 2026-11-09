namespace Orchware.Notification.API.Models.Requests;

public class PagedRequest
{
	public int Take { get; set; }
	public int Skip { get; set; }
	public bool? IsRead { get; set; } = false;
	public PagedFilter? Filter { get; set; }
}

public class PagedFilter
{
	public List<PagedFilterByKeyValue> Filter { get; set; } = new();
	public string Search { get; set; } = string.Empty;
	public string? SortBy { get; set; }
	public bool IsDescending { get; set; } = true;
}

public class PagedFilterByKeyValue
{
	public string Key { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
}

public class PagedResponse<T>
{
	public int Take { get; set; }
	public int Skip { get; set; }
	public PagedFilter? Filter { get; set; }
	public int TotalCount { get; set; }
	public T? Data { get; set; }
}