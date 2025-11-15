namespace MagicMatchTracker.Data.Models;

public sealed class MatchParticipationEndState
{
	public bool IsWinner { get; set; }
	public int? Turn { get; set; }
	public DateTimeOffset? Time { get; set; }
	public LoseCondition? LoseCondition { get; set; }
	public Player? Killer { get; set; }
}