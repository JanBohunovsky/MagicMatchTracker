namespace MagicMatchTracker.Infrastructure.Extensions;

public static class EnumerableExtensions
{
	extension<TSource>(IEnumerable<TSource> source)
	{
		public string JoinToString(string separator)
			=> string.Join(separator, source.Select(i => i?.ToString()));

		public string JoinToString(char separator)
			=> string.Join(separator, source.Select(i => i?.ToString()));

		/// <summary>
		/// Applies the selector to each element of the source and returns the non-null values.
		/// </summary>
		public IEnumerable<TResult> Choose<TResult>(Func<TSource, TResult?> selector) where TResult : class
			=> source.Select(selector).OfType<TResult>();

		/// <summary>
		/// Applies the selector to each element of the source and returns the non-null values.
		/// </summary>
		public IEnumerable<TResult> Choose<TResult>(Func<TSource, TResult?> selector) where TResult : struct
			=> source.Select(selector).OfType<TResult>();
	}
}