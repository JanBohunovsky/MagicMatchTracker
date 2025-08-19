namespace MagicMatchTracker.Infrastructure.Extensions;

public static class EnumerableExtensions
{
	public static string JoinToString<T>(this IEnumerable<T> source, string separator)
		=> string.Join(separator, source.Select(i => i?.ToString()));

	public static string JoinToString<T>(this IEnumerable<T> source, char separator)
		=> string.Join(separator, source.Select(i => i?.ToString()));

	public static bool IsIn<T>(this T item, params IEnumerable<T> values)
		=> values.Contains(item);

	public static bool IsIn<T>(this T item, IEqualityComparer<T> comparer, params IEnumerable<T> values)
		=> values.Contains(item, comparer);

	/// <summary>
	/// Applies the selector to each element of the source and returns the non-null values.
	/// </summary>
	public static IEnumerable<TResult> Choose<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) where TResult : class
		=> source.Select(selector).OfType<TResult>();

	/// <summary>
	/// Applies the selector to each element of the source and returns the non-null values.
	/// </summary>
	public static IEnumerable<TResult> Choose<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) where TResult : struct
		=> source.Select(selector).OfType<TResult>();
}