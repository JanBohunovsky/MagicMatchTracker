namespace MagicMatchTracker.Data.Seed.Parsers;

public interface IReadOnlyGristData
{
	IReadOnlyDictionary<int, Player> Players { get; }
	IReadOnlyDictionary<int, Deck> Decks { get; }
	IReadOnlyDictionary<int, Match> Matches { get; }
	IReadOnlyDictionary<int, MatchParticipation> MatchParticipations { get; }
}