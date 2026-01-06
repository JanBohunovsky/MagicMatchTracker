namespace MagicMatchTracker.Features.Matches.Dialogs.StartTransition;

public sealed class MatchStartTransitionModel(Match model)
{
	private readonly List<Player> _players = model.Participations
		.OrderBy(mp => mp.TurnOrder)
		.ThenBy(mp => mp.Player.Name)
		.Select(mp => mp.Player)
		.ToList();

	public IReadOnlyList<Player> Players => _players;

	public bool IsLive { get; set; } = true;
	public DateOnly Date { get; set; } = DateOnly.Today;
	public int MatchNumber { get; set; } = model.MatchNumber;
	public bool HasTurnOrder { get; set; } = true;

	public void MovePlayerUp(Player player)
	{
		var index = _players.IndexOf(player);
		if (index <= 0)
			return;

		_players[index] = _players[index - 1];
		_players[index - 1] = player;
	}

	public void MovePlayerDown(Player player)
	{
		var index = _players.IndexOf(player);
		if (index < 0 || index >= _players.Count - 1)
			return;

		_players[index] = _players[index + 1];
		_players[index + 1] = player;
	}

	public Match ApplyChanges()
	{
		model.IsLive = IsLive;

		if (IsLive)
		{
			model.TimeStarted = DateTimeOffset.Now;
		}
		else
		{
			model.TimeStarted = Match.GetDateTimeForNonLiveMatch(Date);
			model.MatchNumber = MatchNumber;
		}

		foreach (var participation in model.Participations)
		{
			participation.TurnOrder = HasTurnOrder
				? _players.IndexOf(participation.Player) + 1
				: null;
		}

		return model;
	}
}