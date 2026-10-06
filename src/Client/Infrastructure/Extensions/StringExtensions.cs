using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MagicMatchTracker.Client.Infrastructure.Extensions;

public static class StringExtensions
{
	extension(string value)
	{
		public string ToSlug()
		{
			// 1. Turn all diacritics into their regular characters: e.g. "Sméagol, Helpful Guide" -> "Smeagol, Helpful Guide"
			// 2. Remove non-alphanumeric non-space characters: e.g. "Smeagol, Helpful Guide" -> "Smeagol Helpful Guide"
			// 3. Turn all characters to lowercase: e.g. "Smeagol Helpful Guide" -> "smeagol helpful guide"
			// 4. Replace space with dash: e.g. "smeagol helpful guide" -> "smeagol-helpful-guide"
			var chars = value.Normalize(NormalizationForm.FormD)
				.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
				.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
				.Select(char.ToLowerInvariant)
				.Select(c => char.IsWhiteSpace(c) ? '-' : c)
				.ToArray();

			return new string(chars);
		}

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
}