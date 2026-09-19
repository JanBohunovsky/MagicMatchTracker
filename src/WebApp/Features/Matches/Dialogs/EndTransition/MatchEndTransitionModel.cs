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

		FormattedDuration = match.GetFormattedDuration();
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