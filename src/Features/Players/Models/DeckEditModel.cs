using FluentValidation;

namespace MagicMatchTracker.Features.Players.Models;

public sealed class DeckEditModel
{
	private readonly Deck _model;

	public string Name { get; set; }
	public string Commander { get; set; }
	public string Partner { get; set; }
	public Colours ColourIdentity { get; set; }
	public string ImageUri { get; set; }
	public string DeckUri { get; set; }

	public DeckEditModel(Deck deck)
	{
		_model = deck;
		Name = deck.Name ?? string.Empty;
		Commander = deck.Commander;
		Partner = deck.Partner ?? string.Empty;
		ColourIdentity = deck.ColourIdentity;
		ImageUri = deck.ImageUri ?? string.Empty;
		DeckUri = deck.DeckUri ?? string.Empty;
	}

	public Deck ApplyChanges()
	{
		_model.Name = Name.TrimToNull();
		_model.Commander = Commander.Trim();
		_model.Partner = Partner.TrimToNull();
		_model.ColourIdentity = ColourIdentity;
		_model.ImageUri = ImageUri.TrimToNull();
		_model.DeckUri = DeckUri.TrimToNull();

		return _model;
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