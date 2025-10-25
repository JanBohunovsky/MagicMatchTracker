using System.Diagnostics.CodeAnalysis;

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
		return value.IsNotEmpty() ? value : null;
	}

	public static bool IsEmpty([NotNullWhen(false)] this string? value)
		=> string.IsNullOrEmpty(value);

	public static bool IsNotEmpty([NotNullWhen(true)] this string? value)
		=> !string.IsNullOrEmpty(value);

	public static string ToPossessive(this string value)
		=> value.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) ? $"{value}'" : $"{value}'s";
}