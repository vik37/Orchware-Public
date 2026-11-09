using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations.Shared;

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
