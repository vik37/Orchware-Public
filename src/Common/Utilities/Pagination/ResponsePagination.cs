namespace Utilities.Pagination;

public class ResponsePagination<T> : RequestPagination
	where T : class
{
	public int TotalCount { get; set; }
	public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
	public T? Data { get; set; }
}
