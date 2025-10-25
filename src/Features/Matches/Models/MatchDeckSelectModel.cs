namespace MagicMatchTracker.Features.Matches.Models;

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

	public MatchParticipation ApplyChanges()
	{
		model.Deck = Deck ?? throw new InvalidOperationException("Deck nullability is not allowed yet");

		return model;
	}
}