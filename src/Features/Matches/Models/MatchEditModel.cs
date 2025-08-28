using FluentValidation;

namespace MagicMatchTracker.Features.Matches.Models;

public sealed class MatchEditModel(Match model)
{
	public DateTimeOffset? TimeStarted { get; set; } = model.TimeStarted;
	public DateTimeOffset? TimeEnded { get; set; } = model.TimeEnded;
	public int MatchNumber { get; set; } = model.MatchNumber;
	public string Notes { get; set; } = model.Notes ?? string.Empty;

	public bool HasStarted { get; } = model.TimeStarted is not null;

	public Match ApplyChanges()
	{
		model.TimeStarted = TimeStarted;
		model.TimeEnded = TimeEnded;
		model.MatchNumber = MatchNumber;
		model.Notes = Notes.TrimToNull();

		return model;
	}
}

[UsedImplicitly]
public class MatchEditModelValidator : AbstractValidator<MatchEditModel>
{
	public MatchEditModelValidator()
	{
		RuleFor(m => m.TimeStarted)
			.NotEmpty()
			.When(m => m.HasStarted)
			.WithMessage("Start time is required after the match has started");

		RuleFor(m => m.TimeStarted)
			.NotEmpty()
			.When(m => m.TimeEnded.HasValue && !m.HasStarted)
			.WithMessage("End time requires start time");

		RuleFor(m => m.TimeEnded)
			.GreaterThanOrEqualTo(m => m.TimeStarted)
			.When(m => m.TimeEnded.HasValue && m.TimeStarted.HasValue)
			.WithMessage("End time must be after start time");

		RuleFor(m => m.MatchNumber)
			.GreaterThanOrEqualTo(1)
			.WithMessage("Match number must be a positive number");
	}
}