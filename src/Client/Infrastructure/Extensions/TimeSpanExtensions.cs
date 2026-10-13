namespace MagicMatchTracker.Client.Infrastructure.Extensions;

public static class TimeSpanExtensions
{
	extension(TimeSpan value)
	{
		public string ToShortString()
		{
			if (value.TotalDays >= 1)
				return $"{value.Days}d";

			if (value.TotalHours >= 1)
				return $"{value.Hours}h {value.Minutes}m";

			if (value.TotalMinutes >= 1)
				return $"{value.Minutes}m";

			return "< 1m";
		}
	}
}