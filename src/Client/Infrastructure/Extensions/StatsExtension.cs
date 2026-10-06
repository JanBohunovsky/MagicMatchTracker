namespace MagicMatchTracker.Client.Infrastructure.Extensions;

public static class StatsExtension
{
	private const int MinMatchesForWinRate = 10;

	extension(IStats stats)
	{
		public int? WinRate => stats.Matches >= MinMatchesForWinRate
			? (int)Math.Round(stats.Wins * 100.0 / stats.Matches)
			: null;

		public string FormattedWins => stats.WinRate is not null
			? $"{stats.WinRate}%"
			: stats.Wins.ToString();
	}
}