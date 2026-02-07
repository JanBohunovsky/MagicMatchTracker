namespace MagicMatchTracker.Features.Matches.Dialogs.StartTransition;

public sealed class MatchStartTransitionModel(Match model)
{
	public bool IsLive { get; set; } = true;
	public DateOnly Date { get; set; } = DateOnly.Today;
	public int MatchNumber { get; set; } = model.MatchNumber;
	public bool HasTurnOrder { get; set; } = true;

	public List<Player> Players { get; } = model.Participations
		.OrderBy(mp => mp.TurnOrder)
		.ThenBy(mp => mp.Player.Name)
		.Select(mp => mp.Player)
		.ToList();

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
				? Players.IndexOf(participation.Player) + 1
				: null;
		}

		return model;
	}
}