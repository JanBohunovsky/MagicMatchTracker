using Raffinert.FuzzySharp;
using Raffinert.FuzzySharp.PreProcess;

namespace MagicMatchTracker.Client.Infrastructure.Extensions;

public static class EnumerableExtensions
{
	public static ILogger? FuzzySearchLogger { get; set; }

	extension<T>(IEnumerable<T> source)
	{
		public T? FindBySlug(string? slug, Func<T, string?> slugSelector)
		{
			return source.FindAllBySlug(slug, slugSelector)
				.FirstOrDefault();
		}

		public IEnumerable<T> FindAllBySlug(string? slug, Func<T, string?> slugSelector)
		{
			return source.Where(x => string.Equals(slug, slugSelector(x), StringComparison.OrdinalIgnoreCase));
		}

		public IEnumerable<T> FuzzySearch(string query, params Func<T, string?>[] properties)
		{
			if (query.IsEmpty() || properties.Length == 0)
				return source;

			return source.Select(CalculateItemScore)
				.Where(x => x.Score > 60)
				.OrderByDescending(x => x.Score)
				.Select(x => x.Item);

			(T Item, int Score) CalculateItemScore(T item)
			{
				var bestScore = properties.Select(p => p(item))
					.Select(CalculateValueScore)
					.Max();

				return (item, bestScore);
			}

			int CalculateValueScore(string? value)
			{
				if (value is null)
					return 0;

				var score = Fuzz.WeightedRatio(query, value, PreprocessMode.Full);
				FuzzySearchLogger?.LogInformation("{Value} has score {Score}", value, score);
				return score;
			}
		}
	}
}