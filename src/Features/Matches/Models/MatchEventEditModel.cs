using FluentValidation;

namespace MagicMatchTracker.Features.Matches.Models;

public sealed class MatchEventEditModel
{
	private readonly MatchEvent _model;
	private readonly List<MatchParticipation> _participationsToKill;
	private MatchEventType _type;

	public Player Player => _model.Participation.Player;
	public IReadOnlyList<Player> PlayersInMatch { get; }

	public IEnumerable<Player> PlayersToKill => _participationsToKill.Select(mp => mp.Player);
	public bool CanKillRemainingPlayers => _participationsToKill.Count > 0;

	public MatchEventType Type
	{
		get => _type;
		set
		{
			ResetValuesForEvent(value);
			_type = value;
		}
	}

	public int? Turn { get; set; }
	public DateTimeOffset? Time { get; set; }
	public string Notes { get; set; }
	public LoseCondition? LoseCondition { get; set; }
	public Player? Killer { get; set; }

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
			LoseCondition = data.LoseCondition;
			Killer = PlayersInMatch.FirstOrDefault(p => p.Id == data.KillerId);
		}
	}

	public MatchEvent ApplyChanges()
	{
		if (Type.IsTerminal())
			_model.Participation.IsWinner = Type is MatchEventType.PlayerWon;

		_model.Turn = Turn;

		// For new events, use the current time if it's not set
		if (_model.Id == Guid.Empty && Time is null)
			_model.Time = DateTimeOffset.Now;
		else
			_model.Time = Time;

		_model.Data = GetEventDataForCurrentPlayer();

		if (Type is MatchEventType.PlayerWon && KillRemainingPlayers && LoseCondition is not null)
		{
			ApplyLoseEventToRemainingPlayers(LoseCondition.Value);
		}

		return _model;

		MatchEventData? GetEventDataForCurrentPlayer()
		{
			if (Type is MatchEventType.PlayerLost && LoseCondition is not null && Killer is not null)
			{
				return new PlayerLostEventData
				{
					LoseCondition = LoseCondition.Value,
					KillerId = LoseCondition.Value.GetActualKiller(victim: Player, killer: Killer).Id,
					Notes = Notes.TrimToNull(),
				};
			}

			var notes = Notes.TrimToNull();
			if (notes is not null)
			{
				return new MatchEventData
				{
					Notes = notes,
				};
			}

			return null;
		}

		void ApplyLoseEventToRemainingPlayers(LoseCondition loseCondition)
		{
			foreach (var participation in _participationsToKill)
			{
				var matchEvent = new MatchEvent
				{
					Participation = participation,
					Turn = _model.Turn,
					Time = _model.Time,
					Type = MatchEventType.PlayerLost,
					Data = new PlayerLostEventData
					{
						LoseCondition = loseCondition,
						KillerId = loseCondition.GetActualKiller(victim: participation.Player, killer: Player).Id,
					},
				};
				participation.Events.Add(matchEvent);
			}
		}
	}

	private void ResetValuesForEvent(MatchEventType eventType)
	{
		if (eventType is MatchEventType.PlayerLost)
		{
			LoseCondition = null;
			Killer = null;
		}
		else if (eventType is MatchEventType.PlayerWon)
		{
			KillRemainingPlayers = false;
			LoseCondition = null;
		}
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
			.When(m => m.Type is MatchEventType.PlayerLost && m.LoseCondition is not null && m.LoseCondition != LoseCondition.Concede)
			.WithMessage("Killer is required when lose condition is selected");

		RuleFor(m => m.LoseCondition)
			.NotEmpty()
			.When(m => m.Type is MatchEventType.PlayerLost && m.Killer is not null)
			.WithMessage("Lose condition is required when killer is selected");

		RuleFor(m => m.LoseCondition)
			.NotEmpty()
			.When(m => m.Type is MatchEventType.PlayerWon && m.KillRemainingPlayers)
			.WithMessage("Lose condition is required when kill remaining players is selected");

		RuleFor(m => m.Notes)
			.NotEmpty()
			.When(m => m.LoseCondition == LoseCondition.Other && (m.Type is MatchEventType.PlayerLost || m.KillRemainingPlayers))
			.WithMessage("Describe the lose condition");
	}
}