using FluentValidation;

namespace MagicMatchTracker.Features.Matches.Models;

public sealed class MatchEventEditModel
{
	private readonly MatchEvent _model;
	private readonly List<MatchParticipation> _participationsToKill;

	public Player Player => _model.Participation.Player;
	public IReadOnlyList<Player> PlayersInMatch { get; }

	public IEnumerable<Player> PlayersToKill => _participationsToKill.Select(mp => mp.Player);
	public bool CanKillRemainingPlayers => _participationsToKill.Count > 0;

	public MatchEventType Type { get; set; }
	public int? Turn { get; set; }
	public DateTimeOffset? Time { get; set; }
	public string Notes { get; set; }
	public Player? Killer { get; set; }
	public LoseCondition? LoseCondition { get; set; }

	public bool KillRemainingPlayers { get; set; }

	public MatchEventEditModel(MatchEvent model)
	{
		_model = model;
		Type = model.Type;
		Turn = model.Turn;
		Time = model.Time;
		Notes = model.Data?.Notes ?? string.Empty;

		PlayersInMatch = model.Participation
			.Match
			.Participations
			.Select(mp => mp.Player)
			.ToList();

		_participationsToKill = model.Participation
			.Match
			.Participations
			.Where(mp => mp.Player != model.Participation.Player)
			.Where(mp => mp.Events.All(e => !e.Type.IsTerminal()))
			.ToList();

		if (model.Type is MatchEventType.PlayerLost && model.Data is PlayerLostEventData data)
		{
			Killer = PlayersInMatch.FirstOrDefault(p => p.Id == data.KillerId);
			LoseCondition = data.LoseCondition;
		}
	}

	public MatchEvent ApplyChanges()
	{
		if (Type.IsTerminal())
			_model.Participation.IsWinner = Type is MatchEventType.PlayerWon;

		// TODO: Implement

		// TODO: Handle concede case
		//  - Add validator?
		//  - Automatically set the correct player? Upon save or upon edit?
		//  - Disallow concede and "other" values for winner when "kill remaining players" is checked?

		// TODO: Clear values when switching event type?

		return _model;
	}
}

[UsedImplicitly]
public sealed class MatchEventEditModelValidator : AbstractValidator<MatchEventEditModel>
{
	public MatchEventEditModelValidator()
	{
		RuleFor(m => m.Turn)
			.GreaterThanOrEqualTo(1)
			.WithMessage("Turn must be a positive number");

		RuleFor(m => m.Turn)
			.LessThan(100)
			.WithMessage("Turn must be less than 100");

		RuleFor(m => m.Killer)
			.NotEmpty()
			.When(m => m.Type is MatchEventType.PlayerLost && m.LoseCondition is not null)
			.WithMessage("Killer is required when lose condition is selected");

		RuleFor(m => m.LoseCondition)
			.NotEmpty()
			.When(m => m.Type is MatchEventType.PlayerLost && m.Killer is not null)
			.WithMessage("Lose condition is required when killer is selected");

		RuleFor(m => m.LoseCondition)
			.NotEmpty()
			.When(m => m.Type is MatchEventType.PlayerWon && m.KillRemainingPlayers)
			.WithMessage("Lose condition is required when kill remaining players is selected");
	}
}