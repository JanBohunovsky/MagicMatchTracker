using FluentValidation;

namespace MagicMatchTracker.Features.Players.Dialogs.Edit;

public sealed class PlayerEditModel(Player model)
{
	public string Name { get; set; } = model.Name;
	public string AvatarUri { get; set; } = model.AvatarUri ?? string.Empty;

	public Player ApplyChanges()
	{
		model.Name = Name.Trim();
		model.AvatarUri = AvatarUri.TrimToNull();

		return model;
	}
}

[UsedImplicitly]
public sealed class PlayerEditModelValidator : AbstractValidator<PlayerEditModel>
{
	public PlayerEditModelValidator()
	{
		RuleFor(m => m.Name)
			.NotEmpty()
			.WithMessage("Enter the name");

		RuleFor(m => m.AvatarUri)
			.Must(u => u.StartsWith("https://") || u.StartsWith("http://"))
			.When(m => !string.IsNullOrEmpty(m.AvatarUri))
			.WithMessage("URL starts with http:// or https://");
	}
}