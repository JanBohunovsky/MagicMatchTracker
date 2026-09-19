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

	extension(string value)
	{
		public string ToPossessive()
			=> value.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) ? $"{value}'" : $"{value}'s";

		public string Format(object? arg0)
			=> string.Format(value, arg0);

		public string Format(object? arg0, object? arg1)
			=> string.Format(value, arg0, arg1);

		public string Format(object? arg0, object? arg1, object? arg2)
			=> string.Format(value, arg0, arg1, arg2);

		public string Format(params ReadOnlySpan<object?> args)
			=> string.Format(value, args);

		public string Format(params object[] args)
			=> string.Format(value, args);
	}
}