namespace MagicMatchTracker.Data.Models;

public record Stats(int Matches, int Wins, DateOnly? LastPlayed)
{
	private const int MinimumMatchesForWinRate = 10;

	public static Stats Empty => new(0, 0, null);

	public int? WinRate => Matches >= MinimumMatchesForWinRate
		? (int)Math.Round(Wins * 100.0 / Matches)
		: null;
}

public sealed record PlayerStats(Guid PlayerId, int Matches, int Wins, DateOnly? LastPlayed)
	: Stats(Matches, Wins, LastPlayed);

public sealed record DeckStats(Guid DeckId, int Matches, int Wins, DateOnly? LastPlayed)
	: Stats(Matches, Wins, LastPlayed);