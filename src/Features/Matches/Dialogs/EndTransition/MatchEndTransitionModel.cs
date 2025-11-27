namespace MagicMatchTracker.Features.Matches.Dialogs.EndTransition;

public sealed class MatchEndTransitionModel
{
	private readonly Match _match;

	public Player? Winner { get; }
	public string? FormattedDuration { get; }

	public MatchEndTransitionModel(Match match)
	{
		_match = match;

		Winner = match.Participations
			.Where(mp => mp.EndState?.IsWinner is true)
			.Select(mp => mp.Player)
			.FirstOrDefault();

		var now = DateTimeOffset.Now;
		if (match.IsLive && match.TimeStarted < now)
		{
			FormattedDuration = (now - match.TimeStarted.Value).ToPrettyString().ToLower();
		}
	}

	public Match ApplyChanges()
	{
		if (_match.IsLive)
		{
			_match.TimeEnded = DateTimeOffset.Now;
		}
		else
		{
			var date = _match.TimeStarted!.Value.ToDateOnly();
			_match.TimeEnded = Match.GetDateTimeForNonLiveMatch(date);
		}

		return _match;
	}
}