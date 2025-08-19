namespace MagicMatchTracker.Features.Matches.Models;

public sealed class MatchParticipationEditModel
{
	public MatchEditModel Match { get; }
	public Player? Player { get; set; }
	public Deck? Deck { get; set; }

	public MatchParticipationEditModel(MatchEditModel match, Player? player, Deck? deck)
	{
		Match = match;
		Player = player;
		Deck = deck;
	}
}