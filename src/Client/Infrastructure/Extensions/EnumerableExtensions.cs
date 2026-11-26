using Raffinert.FuzzySharp;
using Raffinert.FuzzySharp.PreProcess;

namespace MagicMatchTracker.Client.Infrastructure.Extensions;

public static class EnumerableExtensions
{
	public static ILogger? FuzzySearchLogger { get; set; }

	extension<T>(IEnumerable<T> source)
	{
		public T? FindById(string? id, Func<T, string?> idSelector)
		{
			return source.FindAllById(id, idSelector)
				.FirstOrDefault();
		}

		public IEnumerable<T> FindAllById(string? id, Func<T, string?> idSelector)
		{
			return source.Where(x => string.Equals(id, idSelector(x), StringComparison.OrdinalIgnoreCase));
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