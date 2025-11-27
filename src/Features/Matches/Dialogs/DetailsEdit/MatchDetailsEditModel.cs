using FluentValidation;

namespace MagicMatchTracker.Features.Matches.Dialogs.DetailsEdit;

public sealed class MatchDetailsEditModel(Match model)
{
	public DateTimeOffset? TimeStarted { get; set; } = model.TimeStarted;
	public DateTimeOffset? TimeEnded { get; set; } = model.TimeEnded;
	public DateOnly? Date { get; set; } = model.TimeStarted?.ToDateOnly();
	public int MatchNumber { get; set; } = model.MatchNumber;
	public string Notes { get; set; } = model.Notes ?? string.Empty;

	public bool IsLive { get; } = model.IsLive;
	public bool HasStarted { get; } = model.HasStarted;
	public bool HasEnded { get; } = model.HasEnded;

	public Match ApplyChanges()
	{
		if (HasStarted)
		{
			model.TimeStarted = IsLive
				? TimeStarted
				: Match.GetDateTimeForNonLiveMatch(Date!.Value);
		}

		if (HasEnded)
		{
			model.TimeEnded = IsLive
				? TimeEnded
				: Match.GetDateTimeForNonLiveMatch(Date!.Value);
		}

		model.MatchNumber = MatchNumber;
		model.Notes = Notes.TrimToNull();

		return model;
	}
}

[UsedImplicitly]
public class MatchDetailsEditModelValidator : AbstractValidator<MatchDetailsEditModel>
{
	public MatchDetailsEditModelValidator()
	{
		RuleFor(m => m.TimeStarted)
			.NotEmpty()
			.When(m => m.IsLive && m.HasStarted)
			.WithMessage("Enter the start time");

		RuleFor(m => m.TimeEnded)
			.NotEmpty()
			.When(m => m.IsLive && m.HasEnded)
			.WithMessage("Enter the end time");

		RuleFor(m => m.TimeEnded)
			.GreaterThanOrEqualTo(m => m.TimeStarted)
			.When(m => m.TimeEnded.HasValue && m.TimeStarted.HasValue)
			.WithMessage("End time must be after start time");

		RuleFor(m => m.MatchNumber)
			.GreaterThanOrEqualTo(1)
			.WithMessage("Match number must be a positive number");
	}
}