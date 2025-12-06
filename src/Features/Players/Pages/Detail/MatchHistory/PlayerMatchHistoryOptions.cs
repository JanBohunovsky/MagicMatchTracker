namespace MagicMatchTracker.Features.Players.Pages.Detail.MatchHistory;

public sealed class PlayerMatchHistoryOptions
{
	public const string SectionName = "PlayerMatchHistory";

	public int MatchesPerPage { get; init; } = 50;
}