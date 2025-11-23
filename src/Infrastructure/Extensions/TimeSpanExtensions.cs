using System.Text;

namespace MagicMatchTracker.Infrastructure.Extensions;

public static class TimeSpanExtensions
{
	public static string ToPrettyString(this TimeSpan timeSpan)
	{
		if (timeSpan.TotalSeconds < 0)
			return "In the future";
		if (timeSpan.TotalMinutes < 1)
			return "Less than a minute";
		if (timeSpan.TotalDays >= 1)
			return "Over a day";

		var sb = new StringBuilder();

		if (timeSpan.Hours > 1)
			sb.Append($"{timeSpan.Hours} hours ");
		else if (timeSpan.Hours > 0)
			sb.Append($"{timeSpan.Hours} hour ");

		if (timeSpan.Minutes > 1)
			sb.Append($"{timeSpan.Minutes} minutes ");
		else if (timeSpan.Minutes > 0)
			sb.Append($"{timeSpan.Minutes} minute ");

		return sb.ToString().Trim();
	}
}