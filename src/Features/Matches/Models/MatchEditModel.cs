namespace MagicMatchTracker.Features.Matches.Models;

public sealed class MatchEditModel
{
	private readonly Match _model;
	private readonly List<MatchParticipationEditModel> _participations;

	public Guid Id => _model.Id;
	public string Title => _model.GetTitle(includeDate: false);

	public IReadOnlyList<MatchParticipationEditModel> Participations => _participations;

	public MatchEditModel(Match match)
	{
		_model = match;
		_participations = match.Participations
			.Select(p => new MatchParticipationEditModel(this, p.Player, p.Deck))
			.Append(new MatchParticipationEditModel(this, null, null))
			.ToList();
	}
}