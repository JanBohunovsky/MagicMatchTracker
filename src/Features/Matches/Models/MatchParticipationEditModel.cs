namespace MagicMatchTracker.Features.Matches.Models;

public sealed class MatchParticipationEditModel
{
	private readonly MatchParticipation? _model;
	private Player? _changedPlayer;
	private Deck? _changedDeck;

	public Match? Match => _model?.Match;

	public Player? Player
	{
		get => _changedPlayer ?? _model?.Player;
		set => _changedPlayer = value;
	}

	public Deck? Deck
	{
		get => _changedDeck ?? _model?.Deck;
		set => _changedDeck = value;
	}

	public MatchParticipationEditModel(MatchParticipation? participation)
	{
		_model = participation;
	}
}