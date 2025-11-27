namespace MagicMatchTracker.Features.Matches.Dialogs.ParticipationDetailsEdit;

public sealed class MatchParticipationDetailsEditModel(MatchParticipation model)
{
	public string Notes { get; set; } = model.Notes ?? string.Empty;

	public Player Player => model.Player;

	public MatchParticipation ApplyChanges()
	{
		model.Notes = Notes.TrimToNull();

		return model;
	}
}