namespace MagicMatchTracker.Features.Matches.Dialogs.DeckSelect;

public sealed class MatchDeckSelectModel(MatchParticipation model)
{
	public Player Player => model.Player;

	public IReadOnlyList<Player> AvailablePlayers { get; } = model.Match
		.Participations
		.Select(mp => mp.Player)
		.OrderByDescending(p => p.Id == model.Player.Id)
		.ThenBy(p => p.Name)
		.ToList();

	public Deck? Deck { get; set; } = model.Deck;

	public bool IsDeckTakenByAnotherPlayer(Deck deck) => model.Match
		.Participations
		.Any(mp => mp.Player != model.Player && mp.Deck == deck);

	public MatchParticipation ApplyChanges()
	{
		model.Deck = Deck;

		return model;
	}
}