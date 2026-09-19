using Humanizer;

namespace MagicMatchTracker.Infrastructure.Extensions;

public static class DateTimeExtensions
{
	public static DateOnly ToDateOnly(this DateTime dateTime)
		=> DateOnly.FromDateTime(dateTime);

	public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset)
		=> dateTimeOffset.Date.ToDateOnly();

	extension(DateOnly date)
	{
		public static DateOnly Today => DateTimeOffset.Now.ToDateOnly();

		public string HumanizeDay(DateOnly? dateToCompareAgainst = null)
		{
			dateToCompareAgainst ??= DateOnly.Today;

			if (date == dateToCompareAgainst)
				return "Today";
			if (date == dateToCompareAgainst.Value.AddDays(-1))
				return "Yesterday";

			return date.Humanize(dateToCompareAgainst);
		}
	}
}