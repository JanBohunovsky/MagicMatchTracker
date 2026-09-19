namespace MagicMatchTracker.Data.Seed;

public interface IImportedData
{
	IReadOnlyCollection<Player> Players { get; }
	IReadOnlyCollection<Deck> Decks { get; }
	IReadOnlyCollection<Match> Matches { get; }
	IReadOnlyCollection<MatchParticipation> MatchParticipations { get; }
}