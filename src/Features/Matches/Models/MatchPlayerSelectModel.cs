using MagicMatchTracker.Infrastructure.Components;

namespace MagicMatchTracker.Features.Matches.Models;

public sealed class MatchPlayerSelectModel(Match model)
{
	private List<CheckableItem<Player>>? _players;

	public IReadOnlyList<CheckableItem<Player>>? Players => _players;

	public void SetAvailablePlayers(IEnumerable<Player> players)
	{
		_players = players.Select(p => new CheckableItem<Player>(p, model.Participations.Any(mp => mp.Player.Id == p.Id)))
			.ToList();
	}

	public Match ApplyChanges()
	{
		if (_players is null)
			return model;

		foreach (var player in _players.Where(i => i.IsChanged))
		{
			if (player.IsChecked)
			{
				model.Participations.Add(new MatchParticipation
				{
					Match = model,
					Player = player.Value,
					Deck = player.Value.Decks.First(), // TODO: Allow null
				});
			}
			else
			{
				model.Participations.RemoveAll(mp => mp.Player.Id == player.Value.Id);
			}
		}
		return model;
	}
}