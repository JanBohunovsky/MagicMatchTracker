using FluentValidation;

namespace MagicMatchTracker.Features.Matches.Dialogs.ParticipationEndStateEdit;

public sealed class MatchParticipationEndStateEditModel
{
	private readonly MatchParticipation _model;
	private readonly List<MatchParticipation> _participationsToKill;

	public bool IsLive => _model.Match.IsLive;
	public bool HasMatchEnded => _model.Match.HasEnded;

	public Player Player => _model.Player;
	public IReadOnlyList<Player> PlayersInMatch { get; }

	public IEnumerable<Player> PlayersToKill => _participationsToKill.Select(mp => mp.Player);
	public bool CanKillRemainingPlayers => _participationsToKill.Count > 0;

	public bool IsWinner
	{
		get;
		set
		{
			LoseCondition = null;
			KillRemainingPlayers = false;
			field = value;
		}
	}

	public LoseCondition? LoseCondition
	{
		get;
		set
		{
			field = value;
			if (field is null || !field.Value.HasKiller)
				Killer = null;
		}
	}

	public int? Turn { get; set; }
	public DateTimeOffset? Time { get; set; }
	public Player? Killer { get; set; }
	public string Notes { get; set; }

	public bool KillRemainingPlayers { get; set; }

	public MatchParticipationEndStateEditModel(MatchParticipation model)
	{
		_model = model;
		_participationsToKill = model.Match
			.Participations
			.Where(mp => mp.Player != model.Player)
			.Where(mp => mp.EndState is null)
			.ToList();

		IsWinner = model.EndState?.IsWinner ?? _participationsToKill.Count == 0;
		Turn = model.EndState?.Turn ?? (IsWinner ? model.Match.GetTotalTurns() : null);
		Time = model.EndState?.Time;
		LoseCondition = model.EndState?.LoseCondition;
		Killer = model.EndState?.Killer;
		Notes = model.Notes ?? string.Empty;

		PlayersInMatch = model.Match
			.Participations
			.Select(mp => mp.Player)
			.ToList();
	}

	public MatchParticipation ApplyChanges()
	{
		var endState = _model.EndState ?? new MatchParticipationEndState();

		if (!HasMatchEnded)
		{
			endState.IsWinner = IsWinner;
		}

		endState.Turn = Turn;

		// For new live results, use the current time if it's not set
		if (!IsLive)
			Time = null;
		else if (_model.EndState is null && Time is null)
			Time = DateTimeOffset.Now;
		endState.Time = Time;

		if (IsWinner)
		{
			endState.LoseCondition = null;
			endState.Killer = null;
		}
		else
		{
			endState.LoseCondition = LoseCondition;
			endState.Killer = LoseCondition?.HasKiller is true ? Killer : null;
		}

		_model.EndState = endState;
		_model.Notes = Notes.TrimToNull();

		if (IsWinner && KillRemainingPlayers)
		{
			ApplyEndStateToRemainingPlayers();
		}

		return _model;

		void ApplyEndStateToRemainingPlayers()
		{
			foreach (var participation in _participationsToKill)
			{
				participation.EndState = new MatchParticipationEndState
				{
					IsWinner = false,
					Turn = Turn,
					Time = Time,
					LoseCondition = LoseCondition,
					Killer = LoseCondition?.HasKiller is true ? Player : null,
				};
			}
		}
	}
}

[UsedImplicitly]
public sealed class MatchEventEditModelValidator : AbstractValidator<MatchParticipationEndStateEditModel>
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
			.When(m => !m.IsWinner && m.LoseCondition is not null && m.LoseCondition.Value.HasKiller)
			.WithMessage("Killer is required for this lose condition");
	}
}