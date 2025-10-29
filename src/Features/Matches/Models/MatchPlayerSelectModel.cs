using MagicMatchTracker.Features.Shared;

namespace MagicMatchTracker.Features.Matches.Models;

public sealed class MatchPlayerSelectModel(Match model)
{
	private readonly HashSet<Player> _selectedPlayers = model.Participations
		.Select(mp => mp.Player)
		.ToHashSet(EntityEqualityComparer<Player>.Default);

	public bool IsPlayerSelected(Player player) => _selectedPlayers.Contains(player);

	public void TogglePlayerSelection(Player player, bool isSelected)
	{
		if (isSelected)
			_selectedPlayers.Add(player);
		else
			_selectedPlayers.Remove(player);
	}

	public Match ApplyChanges()
	{
		var addedPlayers = _selectedPlayers.Except(model.Participations.Select(mp => mp.Player)).ToList();
		var removedParticipations = model.Participations.ExceptBy(_selectedPlayers, mp => mp.Player).ToList();

		foreach (var participation in removedParticipations)
		{
			model.Participations.Remove(participation);
		}

		foreach (var player in addedPlayers)
		{
			model.Participations.Add(new MatchParticipation
			{
				Match = model,
				Player = player,
			});
		}

		return model;
	}
}