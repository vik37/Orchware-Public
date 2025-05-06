namespace DbQueryBuilder.SqlPremmissionProvider;

public class UnsafeSqlFilterException : Exception
{
	public UnsafeSqlFilterException(string message) : base(message){}
}
