namespace MagicMatchTracker.Infrastructure.Extensions;

public static class StringExtensions
{
	/// <summary>
	/// Trims the input string and returns null if the resulting string is empty.
	/// </summary>
	/// <param name="value">The input string to be trimmed.</param>
	/// <returns>The trimmed string, or null if the resulting string is empty.</returns>
	public static string? TrimToNull(this string? value)
	{
		value = value?.Trim();
		return string.IsNullOrEmpty(value) ? null : value;
	}
}