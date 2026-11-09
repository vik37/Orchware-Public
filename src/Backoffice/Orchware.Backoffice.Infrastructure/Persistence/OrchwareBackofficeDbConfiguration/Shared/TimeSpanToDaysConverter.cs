using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.Shared;

public class TimeSpanToDaysConverter : ValueConverter<TimeSpan, int>
{
	public TimeSpanToDaysConverter()
		: base(
				v => (int)v.TotalDays,
				v => TimeSpan.FromDays(v)
			)
		{
	}
}
