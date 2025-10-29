namespace MagicMatchTracker.Data.Models;

public sealed class MatchParticipation
{
	public required Match Match { get; set; }
	public required Player Player { get; set; }
	public Deck? Deck { get; set; }

	public bool IsWinner { get; set; }
	public string? Notes { get; set; }

	public List<MatchEvent> Events { get; set; } = [];
}