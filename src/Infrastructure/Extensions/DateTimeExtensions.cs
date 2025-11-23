namespace MagicMatchTracker.Infrastructure.Extensions;

public static class DateTimeExtensions
{
	public static DateOnly ToDateOnly(this DateTime dateTime)
		=> DateOnly.FromDateTime(dateTime);

	public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset)
		=> dateTimeOffset.Date.ToDateOnly();

	extension(DateOnly)
	{
		public static DateOnly Today => DateTimeOffset.Now.ToDateOnly();
	}
}