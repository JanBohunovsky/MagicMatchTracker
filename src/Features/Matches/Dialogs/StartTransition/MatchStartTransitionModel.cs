namespace MagicMatchTracker.Features.Matches.Dialogs.StartTransition;

public sealed class MatchStartTransitionModel(Match model)
{
	public bool IsLive { get; set; } = true;
	public DateOnly Date { get; set; } = DateOnly.Today;
	public int MatchNumber { get; set; } = model.MatchNumber;

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

		return model;
	}
}