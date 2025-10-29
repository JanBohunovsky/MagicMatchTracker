using FluentValidation;

namespace MagicMatchTracker.Features.Players.Models;

public sealed class DeckEditModel(Deck model)
{
	public string Name { get; set; } = model.Name ?? string.Empty;
	public string Commander { get; set; } = model.Commander;
	public string Partner { get; set; } = model.Partner ?? string.Empty;
	public Colours ColourIdentity { get; set; } = model.ColourIdentity;
	public string ImageUri { get; set; } = model.ImageUri ?? string.Empty;
	public string DeckUri { get; set; } = model.DeckUri ?? string.Empty;
	public bool IsArchived { get; set; } = model.IsArchived;

	public Deck ApplyChanges()
	{
		model.Name = Name.TrimToNull();
		model.Commander = Commander.Trim();
		model.Partner = Partner.TrimToNull();
		model.ColourIdentity = ColourIdentity;
		model.ImageUri = ImageUri.TrimToNull();
		model.DeckUri = DeckUri.TrimToNull();
		model.IsArchived = IsArchived;

		return model;
	}
}

[UsedImplicitly]
public class DeckEditModelValidator : AbstractValidator<DeckEditModel>
{
	public DeckEditModelValidator()
	{
		RuleFor(m => m.Commander)
			.NotEmpty()
			.WithMessage("Enter the commander");

		RuleFor(m => m.ImageUri)
			.Must(u => u.StartsWith("https://") || u.StartsWith("http://"))
			.When(m => !string.IsNullOrEmpty(m.ImageUri))
			.WithMessage("URL starts with http:// or https://");

		RuleFor(m => m.DeckUri)
			.Must(u => u.StartsWith("https://") || u.StartsWith("http://"))
			.When(m => !string.IsNullOrEmpty(m.DeckUri))
			.WithMessage("URL starts with http:// or https://");
	}
}